using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using PagamentoEntity = LiveEventsTicket.Backend.Modules.Pagamento.Model.Pagamento;

namespace LiveEventsTicket.Backend.Modules.Pagamento.Service;

public class PagamentoService
{
    // --- PROCESSAR PAGAMENTO ---
    public PagamentoEntity ProcessarPagamento(CriarPedidoDto dto, int pedidoId)
    {
        var tipo = dto.Pagamento.Tipo.ToUpperInvariant();

        // --- PAGAMENTO VIA CARTÃO ---
        if (tipo == "CARTAO")
        {
            var numero = dto.Pagamento.NumeroCartao ?? string.Empty;
            var ultimoDigito = numero.LastOrDefault();
            var aprovado = char.IsDigit(ultimoDigito) && ((ultimoDigito - '0') % 2 == 0);

            return new PagamentoEntity
            {
                PedidoId = pedidoId,
                Tipo = "CARTAO",
                Status = aprovado ? "APROVADO" : "RECUSADO"
            };
        }

        // --- PAGAMENTO VIA PIX ---
        return new PagamentoEntity
        {
            PedidoId = pedidoId,
            Tipo = "PIX",
            Status = "APROVADO",
            CodigoPix = $"PIX-{Guid.NewGuid():N}"[..20]
        };
    }
}
