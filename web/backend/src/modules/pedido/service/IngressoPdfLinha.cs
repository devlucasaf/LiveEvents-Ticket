namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class IngressoPdfLinha
{
    public string 	EventoTitulo 			{ get; set; } = string.Empty;
    public string 	EventoData 				{ get; set; } = string.Empty;
    public string 	EventoLocal 			{ get; set; } = string.Empty;
    public string 	Setor 					{ get; set; } = string.Empty;
    public int 		Quantidade 				{ get; set; }
    public string 	ValorUnitarioFormatado 	{ get; set; } = string.Empty;
    public string 	SubtotalFormatado 		{ get; set; } = string.Empty;
}
