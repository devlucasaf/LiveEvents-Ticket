using LiveEventsTicket.Backend.Modules.Evento.Repository;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;

using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

// --- GERA O PDF DO INGRESSO PARA O DONO OU VIA LINK COMPARTILHADO ---
public class IngressoPdfService
{
    private readonly IEventoRepository   _eventoRepository;
    private readonly IIngressoRepository _ingressoRepository;
    private readonly IPedidoRepository   _pedidoRepository;

    public IngressoPdfService(
        IEventoRepository eventoRepository,
        IIngressoRepository ingressoRepository,
        IPedidoRepository pedidoRepository)
    {
        _eventoRepository   = eventoRepository;
        _ingressoRepository = ingressoRepository;
        _pedidoRepository   = pedidoRepository;
    }

    // --- GERA PDF DO INGRESSO PARA O USUARIO DONO DO PEDIDO ---
    public async Task<(byte[] arquivo, string nomeArquivo)> GerarPeloDonoAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        if (pedido.Status != StatusPedido.Pago)
        {
            throw new InvalidOperationException("O PDF do ingresso está disponível apenas para pedidos pagos.");
        }

        return await MontarAsync(pedido, cancellationToken);
    }

    // --- BAIXA PDF A PARTIR DE TOKEN PUBLICO TEMPORARIO ---
    public async Task<(byte[] arquivo, string nomeArquivo)> BaixarCompartilhadoAsync(
        string tokenCompartilhamento,
        CancellationToken cancellationToken = default)
    {
        var token = (tokenCompartilhamento ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Token de compartilhamento inválido.");
        }

        // --- BUSCA PEDIDO PELO TOKEN E VALIDA STATUS/EXPIRACAO ---
        var pedido = await _pedidoRepository.BuscarPorCompartilhamentoTokenAsync(token, cancellationToken)
            ?? throw new KeyNotFoundException("Link de compartilhamento não encontrado.");

        if (pedido.Status != StatusPedido.Pago)
        {
            throw new InvalidOperationException("Este ingresso não está disponível para compartilhamento.");
        }

        if (!PedidoHelpers.CompartilhamentoAtivo(pedido, DateTime.UtcNow))
        {
            throw new InvalidOperationException("Link expirado, revogado ou limite de acessos atingido.");
        }

        // --- GERA O PDF E INCREMENTA CONTADOR DE ACESSOS ---
        var resultado = await MontarAsync(pedido, cancellationToken);

        pedido.CompartilhamentoAcessosRealizados += 1;
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return resultado;
    }

    // --- MONTA O PDF DO INGRESSO COM DADOS COMPLETOS DO PEDIDO ---
    private async Task<(byte[] arquivo, string nomeArquivo)> MontarAsync(
        PedidoEntity pedido,
        CancellationToken cancellationToken)
    {
        var linhas = new List<IngressoPdfLinha>();

        foreach (var item in pedido.Itens)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(item.IngressoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Ingresso {item.IngressoId} não encontrado.");

            var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);

            linhas.Add(new IngressoPdfLinha
            {
                EventoTitulo            = evento?.Titulo ?? "Evento não identificado",
                EventoData              = evento?.DataEvento.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? "Data não informada",
                EventoLocal             = evento?.Local ?? "Local não informado",
                Setor                   = ingresso.Setor,
                Quantidade              = item.Quantidade,
                ValorUnitarioFormatado  = PedidoHelpers.FormatarMoeda(item.PrecoUnitario),
                SubtotalFormatado       = PedidoHelpers.FormatarMoeda(item.PrecoUnitario * item.Quantidade)
            });
        }

        var endereco = PedidoHelpers.MontarEnderecoComprador(pedido);

        // --- GARANTE QUE EXISTA UM TOKEN DE CHECKIN PERSISTIDO PARA O QR ---
        if (string.IsNullOrWhiteSpace(pedido.CheckinToken))
        {
            pedido.CheckinToken = PedidoHelpers.GerarTokenCheckin();
            await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        // --- REUTILIZA O QR ARMAZENADO OU GERA NA HORA ---
        var qrCode = string.IsNullOrWhiteSpace(pedido.QrCodeBase64)
            ? PedidoHelpers.GerarQrCodeBase64(pedido.CheckinToken)
            : pedido.QrCodeBase64;

        var dados = new IngressoPdfDados
        {
            PedidoId            = pedido.Id,
            NomeComprador       = pedido.CompradorNome,
            DocumentoComprador  = pedido.CompradorCpf,
            EmailComprador      = pedido.CompradorEmail,
            EnderecoComprador   = endereco,
            DataCompra          = pedido.DataCriacao.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            ValorTotalFormatado = PedidoHelpers.FormatarMoeda(pedido.ValorTotal),
            QrCodeBase64        = qrCode,
            Itens               = linhas
        };

        var pdf         = IngressoPdfBuilder.Gerar(dados);
        var nomeArquivo = $"ingresso-pedido-{pedido.Id}.pdf";

        return (pdf, nomeArquivo);
    }
}
