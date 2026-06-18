namespace LiveEventsTicket.Backend.Modules.Pedido.Model;

public class Pedido
{
    public int                  Id              { get; set; }
    public int                  UsuarioId       { get; set; }
    public decimal              ValorTotal      { get; set; }
    public string               Status          { get; set; } = "CRIADO";
    public string?              QrCodeBase64    { get; set; }
    public DateTime             DataCriacao     { get; set; } = DateTime.UtcNow;
    public List<ItemPedido>     Itens           { get; set; } = [];
}
