namespace LiveEventsTicket.Backend.Modules.Pedido.Model;

public class ItemPedido
{
    public int      Id              { get; set; }
    public int      PedidoId        { get; set; }
    public int      IngressoId      { get; set; }
    public int      Quantidade      { get; set; }
    public decimal  PrecoUnitario   { get; set; }
    public string   Modalidade      { get; set; } = "INTEIRA";
    public string?  Subtipo         { get; set; }
    public string?  DocumentosJson  { get; set; }
}
