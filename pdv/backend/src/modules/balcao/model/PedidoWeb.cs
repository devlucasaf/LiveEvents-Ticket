namespace PontoVenda.Backend.Modules.Balcao.Model;

public class PedidoWeb
{
    public int                  Id              { get; set; }
    public int                  UsuarioId       { get; set; }
    public decimal              ValorTotal      { get; set; }
    public string               Status          { get; set; } = "CRIADO";
    public string?              QrCodeBase64    { get; set; }
    public DateTime             DataCriacao     { get; set; } = DateTime.UtcNow;
    public List<ItemPedidoWeb>  Itens           { get; set; } = new();
}
