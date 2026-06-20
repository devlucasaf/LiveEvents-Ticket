namespace LiveEventsTicket.Backend.Exception;

public class ErrorResponse
{
    public string   Message     { get; set; } = string.Empty;
    public int      StatusCode  { get; set; }
    public DateTime Timestamp   { get; set; } = DateTime.UtcNow;
}
