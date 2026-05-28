namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class PedidoRespostaDto
{
    public int      Id              { get; set; }
    public decimal  ValorTotal      { get; set; }
    public string   Status          { get; set; } = string.Empty;
    public string?  QrCodeBase64    { get; set; }
    public string   PagamentoStatus { get; set; } = string.Empty;
    public string?  CodigoPix       { get; set; }
}
