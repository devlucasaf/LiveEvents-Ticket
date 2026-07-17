namespace PontoVenda.Backend.Modules.Balcao.Model;

public class ItemPedidoWeb
{
    public int      Id              { get; set; }
    public int      PedidoId        { get; set; }
    public int      IngressoId      { get; set; }
    public int      Quantidade      { get; set; }
    public decimal  PrecoUnitario   { get; set; }
}
