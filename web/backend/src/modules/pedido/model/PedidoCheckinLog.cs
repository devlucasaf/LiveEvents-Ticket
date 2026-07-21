namespace LiveEventsTicket.Backend.Modules.Pedido.Model;

public class PedidoCheckinLog
{
    public int 		Id 				{ get; set; }
    public int? 	PedidoId 		{ get; set; }
    public int 		OperadorId 		{ get; set; }
    public string 	TokenInformado 	{ get; set; } = string.Empty;
    public bool 	Permitido 		{ get; set; }
    public string 	Mensagem 		{ get; set; } = string.Empty;
    public DateTime DataCheckin 	{ get; set; } = DateTime.UtcNow;
}
