namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class ItemPedidoDto
{
    public int                                  IngressoId  { get; set; }
    public int                                  Quantidade  { get; set; }
    public string                               Modalidade  { get; set; } = "INTEIRA";
    public string?                              SubtipoMeia { get; set; }
    public List<Dictionary<string, string?>>?   Documentos { get; set; }
}
