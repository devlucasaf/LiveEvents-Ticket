using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class VendaFisica : AuditEntity
{
    public Guid             Id                  { get; set; } = Guid.NewGuid();
    public Guid             EventoId            { get; set; }
    public Guid             AssentoId           { get; set; }
    public int              OperadorId          { get; set; }
    public MetodoPagamento  MetodoPagamento     { get; set; }
    public decimal          Valor               { get; set; }
    public DateTime         DataVenda           { get; set; } = DateTime.UtcNow;
}
