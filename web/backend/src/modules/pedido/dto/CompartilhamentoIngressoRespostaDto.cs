namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class CompartilhamentoIngressoRespostaDto
{
    public int 			PedidoId 			{ get; set; }
    public string 		Token 				{ get; set; } = string.Empty;
    public string 		UrlCompartilhamento { get; set; } = string.Empty;
    public DateTime 	ExpiraEm 			{ get; set; }
    public DateTime? 	RevogadoEm 			{ get; set; }
    public int 			MaxAcessos 			{ get; set; }
    public int 			AcessosRealizados 	{ get; set; }
    public bool 		Ativo 				{ get; set; }
    public string 		Mensagem 			{ get; set; } = string.Empty;
}
