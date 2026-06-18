using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class Evento : AuditEntity
{
    public Guid     Id              { get; set; } = Guid.NewGuid();
    public string   Nome            { get; set; } = string.Empty;
    public string   Local           { get; set; } = string.Empty;
    public DateTime DataEvento      { get; set; }
    public string?  Descricao       { get; set; }
    public bool     Ativo           { get; set; } = true;
    public ICollection<Assento> Assentos { get; set; } = new List<Assento>();
}
