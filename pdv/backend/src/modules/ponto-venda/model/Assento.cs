using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class Assento : AuditEntity
{
    public Guid     Id          { get; set; } = Guid.NewGuid();
    public Guid     EventoId    { get; set; }
    public string   Setor       { get; set; } = string.Empty;  
    public string   Fileira     { get; set; } = string.Empty;   
    public int      Numero      { get; set; }
    public decimal  Preco       { get; set; }
    public string   Status      { get; set; } = "DISPONIVEL";
}
