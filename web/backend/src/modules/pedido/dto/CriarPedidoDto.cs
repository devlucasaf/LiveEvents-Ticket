namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class CriarPedidoDto
{
    public List<ItemPedidoDto> Itens { get; set; } = [];
    public DadosPagamentoDto Pagamento { get; set; } = new();
}
