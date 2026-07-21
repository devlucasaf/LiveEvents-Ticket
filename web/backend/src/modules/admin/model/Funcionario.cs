namespace LiveEventsTicket.Backend.Modules.Admin.Model;

public class Funcionario
{
    public int          Id          { get; set; }
    public string       Nome        { get; set; } = string.Empty;
    public string       Login       { get; set; } = string.Empty;
    public string       SenhaHash   { get; set; } = string.Empty;
    public string       Role        { get; set; } = "OPERADOR";
    public bool         Ativo       { get; set; } = true;
    public DateTime     CreatedAt   { get; set; } = DateTime.UtcNow;
    public DateTime?    UpdatedAt   { get; set; }
}
