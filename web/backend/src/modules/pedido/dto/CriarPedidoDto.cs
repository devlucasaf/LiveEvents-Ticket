namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class CriarPedidoDto
{
    public List<ItemPedidoDto> Itens { get; set; } = [];
    public DadosCompradorDto Comprador { get; set; } = new();
    public DadosPagamentoDto Pagamento { get; set; } = new();
}
