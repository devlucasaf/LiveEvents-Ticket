using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class VendaFisica : AuditEntity
{
    public Guid     Id              { get; set; } = Guid.NewGuid();
    public Guid     OperadorId      { get; set; }
    public Guid     EventoId        { get; set; }
    public decimal  ValorTotal      { get; set; }
    public string   FormaPagamento  { get; set; } = "DINHEIRO";
    public string   Status          { get; set; } = "CONCLUIDA";

    public DateTime DataVenda       { get; set; } = DateTime.UtcNow;
    public string?  NomeComprador   { get; set; }
    public string?  CpfComprador    { get; set; }
    public ICollection<ItemVendaFisica> Itens { get; set; } = new List<ItemVendaFisica>();
}
