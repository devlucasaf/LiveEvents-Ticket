namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class CheckinRespostaDto
{
    public bool 	Permitido 		{ get; set; }
    public string 	Mensagem 		{ get; set; } = string.Empty;
    public int? 	PedidoId 		{ get; set; }
    public string? 	EventoTitulo 	{ get; set; }
    public string? 	Setor 			{ get; set; }
    public int 		QuantidadeTotal { get; set; }
    public int 		UsosRealizados 	{ get; set; }
    public int 		UsosRestantes 	{ get; set; }
    public DateTime DataCheckin 	{ get; set; }
}
