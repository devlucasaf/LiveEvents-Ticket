using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class Venda : AuditEntity
{
    public int              Id              { get; set; }
    public int              OperadorId      { get; set; }
    public decimal          ValorTotal      { get; set; }
    public string           FormaPagamento  { get; set; } = "DINHEIRO";
    public string           Status          { get; set; } = "CONCLUIDA";
    public DateTime         DataVenda       { get; set; } = DateTime.UtcNow;
    public List<ItemVenda>  Itens           { get; set; } = [];
}
