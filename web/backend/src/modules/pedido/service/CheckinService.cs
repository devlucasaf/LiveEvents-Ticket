using LiveEventsTicket.Backend.Modules.Evento.Repository;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;

using PedidoCheckinLogEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.PedidoCheckinLog;
using PedidoEntity           = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class CheckinService
{
    private readonly IEventoRepository _eventoRepository;
    private readonly IIngressoRepository _ingressoRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public CheckinService(
        IEventoRepository eventoRepository,
        IIngressoRepository ingressoRepository,
        IPedidoRepository pedidoRepository)
    {
        _eventoRepository   = eventoRepository;
        _ingressoRepository = ingressoRepository;
        _pedidoRepository   = pedidoRepository;
    }

    // --- VALIDA O TOKEN DO INGRESSO E BLOQUEIA REUTILIZACAO ALEM DO LIMITE ---
    public async Task<CheckinRespostaDto> ValidarAsync(
        int operadorId,
        ValidarCheckinDto dto,
        CancellationToken cancellationToken = default)
    {
        var token = (dto.Token ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return await RegistrarTentativaAsync(
                pedido: null,
                operadorId: operadorId,
                tokenInformado: token,
                permitido: false,
                mensagem: "Token do ingresso não informado.",
                cancellationToken: cancellationToken);
        }

        // --- LOCALIZA O PEDIDO PELO TOKEN ---
        var pedido = await _pedidoRepository.BuscarPorCheckinTokenAsync(token, cancellationToken);
        if (pedido is null)
        {
            return await RegistrarTentativaAsync(
                pedido: null,
                operadorId: operadorId,
                tokenInformado: token,
                permitido: false,
                mensagem: "Ingresso inválido ou não encontrado.",
                cancellationToken: cancellationToken);
        }

        // --- CALCULA A QUANTIDADE TOTAL DE INGRESSOS DO PEDIDO ---
        var quantidadeTotal = pedido.Itens.Sum(i => i.Quantidade);
        if (quantidadeTotal <= 0)
        {
            return await RegistrarTentativaAsync(
                pedido, operadorId, token, false,
                "Pedido sem itens válidos para check-in.",
                cancellationToken, quantidadeTotal, pedido.CheckinUsosRealizados);
        }

        // --- SO PERMITE CHECKIN EM PEDIDOS PAGOS ---
        if (pedido.Status != StatusPedido.Pago)
        {
            return await RegistrarTentativaAsync(
                pedido, operadorId, token, false,
                "Check-in disponível apenas para pedidos pagos.",
                cancellationToken, quantidadeTotal, pedido.CheckinUsosRealizados);
        }

        // --- BLOQUEIA REUTILIZACAO APOS ATINGIR O LIMITE ---
        if (pedido.CheckinUsosRealizados >= quantidadeTotal)
        {
            return await RegistrarTentativaAsync(
                pedido, operadorId, token, false,
                "Todos os ingressos deste pedido já foram utilizados.",
                cancellationToken, quantidadeTotal, pedido.CheckinUsosRealizados);
        }

        // --- INCREMENTA USO E PERSISTE ---
        pedido.CheckinUsosRealizados += 1;
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return await RegistrarTentativaAsync(
            pedido, operadorId, token, true,
            "Check-in validado com sucesso.",
            cancellationToken, quantidadeTotal, pedido.CheckinUsosRealizados);
    }

    // --- REGISTRA A TENTATIVA DE CHECKIN E RETORNA A RESPOSTA PADRONIZADA ---
    private async Task<CheckinRespostaDto> RegistrarTentativaAsync(
        PedidoEntity? pedido,
        int operadorId,
        string tokenInformado,
        bool permitido,
        string mensagem,
        CancellationToken cancellationToken,
        int quantidadeTotal = 0,
        int usosRealizados = 0)
    {
        // --- REGISTRA O LOG DA TENTATIVA ---
        var log = new PedidoCheckinLogEntity
        {
            PedidoId        = pedido?.Id,
            OperadorId      = operadorId,
            TokenInformado  = tokenInformado,
            Permitido       = permitido,
            Mensagem        = mensagem,
            DataCheckin     = DateTime.UtcNow
        };

        await _pedidoRepository.AdicionarCheckinLogAsync(log, cancellationToken);

        // --- ENRIQUECE A RESPOSTA COM EVENTO/SETOR QUANDO POSSIVEL ---
        string? eventoTitulo = null;
        string? setor        = null;

        if (pedido is not null && pedido.Itens.Count > 0)
        {
            var primeiroItem = pedido.Itens[0];
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(primeiroItem.IngressoId, cancellationToken);
            if (ingresso is not null)
            {
                setor = ingresso.Setor;
                var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);
                eventoTitulo = evento?.Titulo;
            }
        }

        return new CheckinRespostaDto
        {
            Permitido       = permitido,
            Mensagem        = mensagem,
            PedidoId        = pedido?.Id,
            EventoTitulo    = eventoTitulo,
            Setor           = setor,
            QuantidadeTotal = quantidadeTotal,
            UsosRealizados  = usosRealizados,
            UsosRestantes   = Math.Max(0, quantidadeTotal - usosRealizados),
            DataCheckin     = log.DataCheckin
        };
    }
}
