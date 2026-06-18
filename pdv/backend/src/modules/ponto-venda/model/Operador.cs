using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class Operador : AuditEntity
{
    public int      Id          { get; set; }
    public string   Nome        { get; set; } = string.Empty;
    public string   Login       { get; set; } = string.Empty;
    public string   SenhaHash   { get; set; } = string.Empty;
    public string   Role        { get; set; } = "OPERADOR";
}
