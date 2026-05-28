namespace LiveEventsTicket.Backend.Infra.Security;

public class JwtOptions
{
    public string   Secret          { get; set; } = "change-me";
    public string   Issuer          { get; set; } = "LiveEventsTicket";
    public string   Audience        { get; set; } = "LiveEventsTicketClient";
    public int      ExpiryMinutes   { get; set; } = 120;
}
