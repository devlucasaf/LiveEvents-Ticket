namespace PontoVenda.Backend.Infra.Security;

public class JwtOptions
{
    public string   Secret          { get; set; } = "change-me";
    public string   Issuer          { get; set; } = "PontoVenda";
    public string   Audience        { get; set; } = "PontoVendaClient";
    public int      ExpiryMinutes   { get; set; } = 120;
}
