using LiveEventsTicket.Backend.Modules.Evento.Repository;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;

using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class ReembolsoService
{
    private readonly IEventoRepository _eventoRepository;
    private readonly IIngressoRepository _ingressoRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public ReembolsoService(
        IEventoRepository eventoRepository,
        IIngressoRepository ingressoRepository,
        IPedidoRepository pedidoRepository)
    {
        _eventoRepository = eventoRepository;
        _ingressoRepository = ingressoRepository;
        _pedidoRepository = pedidoRepository;
    }

    // --- SOLICITA REEMBOLSO COM DECISAO AUTOMATICA E SEM APROVACAO MANUAL ---
    public async Task<ReembolsoRespostaDto> SolicitarAsync(
        int usuarioId,
        int pedidoId,
        SolicitarReembolsoDto dto,
        CancellationToken cancellationToken = default)
    {
        // --- CARREGA O PEDIDO GARANTINDO QUE PERTENCE AO USUARIO ---
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        var pagamento = await _pedidoRepository.BuscarPagamentoPorPedidoIdAsync(pedidoId, cancellationToken)
            ?? throw new InvalidOperationException("Pagamento do pedido não encontrado.");

        // --- CARREGA OS INGRESSOS DOS ITENS PARA REPOR ESTOQUE ---
        var ingressoIds = pedido.Itens.Select(i => i.IngressoId).Distinct().ToList();
        var ingressos   = new Dictionary<int, IngressoEntity>();
        foreach (var ingressoId in ingressoIds)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(ingressoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Ingresso {ingressoId} não encontrado.");

            ingressos[ingressoId] = ingresso;
        }

        // --- DESCOBRE DATA DO EVENTO PARA AVALIACAO DE ELEGIBILIDADE ---
        DateTime? dataEvento = null;
        var primeiroIngresso = pedido.Itens.FirstOrDefault();
        if (primeiroIngresso is not null && ingressos.TryGetValue(primeiroIngresso.IngressoId, out var primeiro))
        {
            var evento = await _eventoRepository.BuscarPorIdAsync(primeiro.EventoId, cancellationToken);
            dataEvento = evento?.DataEvento;
        }

        // --- AVALIA ELEGIBILIDADE USANDO REGRAS CENTRALIZADAS ---
        var agora = DateTime.UtcNow;
        var (elegivel, mensagem, regra) = ReembolsoRegras.AvaliarElegibilidade(pedido, dataEvento, agora);
        if (!elegivel)
        {
            throw new InvalidOperationException(mensagem ?? "Pedido não elegível para reembolso automático.");
        }

        // --- NORMALIZA E VALIDA O MOTIVO INFORMADO ---
        var motivo = ReembolsoRegras.NormalizarMotivo(dto);

        // --- REPOE ESTOQUE DOS INGRESSOS ---
        foreach (var item in pedido.Itens)
        {
            if (ingressos.TryGetValue(item.IngressoId, out var ingresso))
            {
                ingresso.QuantidadeDisponivel += item.Quantidade;
            }
        }

        // --- ATUALIZA STATUS DE PAGAMENTO E PEDIDO ---
        pagamento.Status                = StatusPagamento.Reembolsado;
        pagamento.DataPagamento         = agora;

        pedido.Status                   = StatusPedido.Reembolsado;
        pedido.ReembolsoSolicitadoEm    = agora;
        pedido.ReembolsoAprovadoEm      = agora;
        pedido.ReembolsoMotivoCodigo    = motivo.Codigo;
        pedido.ReembolsoMotivo          = motivo.MotivoPersistido;
        pedido.ReembolsoRegraAplicada   = regra;

        await _ingressoRepository.AtualizarAsync(cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        var protocoloEstorno = PedidoHelpers.GerarProtocoloEstorno(pedido.Id, pagamento.DataPagamento);

        return new ReembolsoRespostaDto
        {
            PedidoId              = pedido.Id,
            Status                = pedido.Status,
            Mensagem              = "Reembolso aprovado automaticamente e processado com sucesso.",
            RegraAplicada         = regra,
            MotivoCodigo          = motivo.Codigo,
            MotivoDescricao       = motivo.Descricao,
            MotivoDetalhe         = motivo.Detalhe,
            SolicitadoEm          = pedido.ReembolsoSolicitadoEm,
            AprovadoEm            = pedido.ReembolsoAprovadoEm,
            EstornadoEm           = pagamento.DataPagamento,
            ProtocoloEstorno      = protocoloEstorno,
            ComprovanteDisponivel = true
        };
    }

    // --- GERA COMPROVANTE DE ESTORNO EM PDF PARA PEDIDOS REEMBOLSADOS ---
    public async Task<(byte[] arquivo, string nomeArquivo)> GerarComprovanteEstornoPdfAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        var pagamento = await _pedidoRepository.BuscarPagamentoPorPedidoIdAsync(pedidoId, cancellationToken)
            ?? throw new InvalidOperationException("Pagamento do pedido não encontrado.");

        if (pedido.Status != StatusPedido.Reembolsado
            || pagamento.Status != StatusPagamento.Reembolsado)
        {
            throw new InvalidOperationException("Comprovante disponível apenas para pedidos estornados.");
        }

        // --- CARREGA DADOS DO EVENTO/SETOR PARA COMPOR O PDF ---
        var primeiroItem       = pedido.Itens.FirstOrDefault();
        string eventoTitulo    = "Evento não identificado";
        string? setor          = null;
        DateTime? dataEvento   = null;

        if (primeiroItem is not null)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(primeiroItem.IngressoId, cancellationToken);
            if (ingresso is not null)
            {
                setor = ingresso.Setor;
                var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);
                if (evento is not null)
                {
                    eventoTitulo = evento.Titulo;
                    dataEvento   = evento.DataEvento;
                }
            }
        }

        // --- MONTA O DTO DE DADOS DO PDF DE ESTORNO ---
        var estornadoEm = pagamento.DataPagamento;
        var dados = new EstornoComprovantePdfDados
        {
            PedidoId                = pedido.Id,
            ProtocoloEstorno        = PedidoHelpers.GerarProtocoloEstorno(pedido.Id, estornadoEm),
            NomeComprador           = pedido.CompradorNome,
            DocumentoComprador      = pedido.CompradorCpf,
            EmailComprador          = pedido.CompradorEmail,
            MotivoCodigo            = pedido.ReembolsoMotivoCodigo ?? MotivoReembolso.Outro,
            MotivoDescricao         = ReembolsoRegras.DescricaoDoCodigo(pedido.ReembolsoMotivoCodigo),
            MotivoInformado         = pedido.ReembolsoMotivo,
            RegraAplicada           = pedido.ReembolsoRegraAplicada,
            ValorEstornadoFormatado = PedidoHelpers.FormatarMoeda(pedido.ValorTotal),
            DataSolicitacao         = pedido.ReembolsoSolicitadoEm,
            DataAprovacao           = pedido.ReembolsoAprovadoEm,
            DataEstorno             = estornadoEm,
            EventoTitulo            = eventoTitulo,
            EventoData              = dataEvento,
            Setor                   = setor,
            QuantidadeIngressos     = pedido.Itens.Sum(i => i.Quantidade)
        };

        var pdf         = EstornoComprovantePdfBuilder.Gerar(dados);
        var nomeArquivo = $"comprovante-estorno-pedido-{pedido.Id}.pdf";
        return (pdf, nomeArquivo);
    }
}
