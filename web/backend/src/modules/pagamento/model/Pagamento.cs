namespace LiveEventsTicket.Backend.Modules.Pagamento.Model;

public class Pagamento
{
    public int      Id              { get; set; }
    public int      PedidoId        { get; set; }
    public string   Tipo            { get; set; } = string.Empty;
    public string   Status          { get; set; } = "PENDENTE";
    public string?  CodigoPix       { get; set; }
    public DateTime DataPagamento   { get; set; } = DateTime.UtcNow;
}
