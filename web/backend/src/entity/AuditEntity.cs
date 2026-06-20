namespace LiveEventsTicket.Backend.Entity;

public abstract class AuditEntity
{
    public DateTime  CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
