namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class ReembolsoRespostaDto
{
    public int 			PedidoId 				{ get; set; }
    public string 		Status 					{ get; set; } = string.Empty;
    public string 		Mensagem 				{ get; set; } = string.Empty;
    public string? 		RegraAplicada 			{ get; set; }
    public string 		MotivoCodigo 			{ get; set; } = string.Empty;
    public string 		MotivoDescricao 		{ get; set; } = string.Empty;
    public string? 		MotivoDetalhe 			{ get; set; }
    public DateTime? 	SolicitadoEm 			{ get; set; }
    public DateTime? 	AprovadoEm 				{ get; set; }
    public DateTime? 	EstornadoEm 			{ get; set; }
    public string 		ProtocoloEstorno 		{ get; set; } = string.Empty;
    public bool 		ComprovanteDisponivel 	{ get; set; }
}
