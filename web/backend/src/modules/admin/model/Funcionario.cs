using LiveEventsTicket.Backend.Entity;

namespace LiveEventsTicket.Backend.Modules.Admin.Model;

public class Funcionario : AuditEntity
{
    public int          Id          { get; set; }
    public string       Nome        { get; set; } = string.Empty;
    public string       Login       { get; set; } = string.Empty;
    public string       SenhaHash   { get; set; } = string.Empty;
    public string       Role        { get; set; } = "OPERADOR";
    public bool         Ativo       { get; set; } = true;
}
