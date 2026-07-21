namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class EstornoComprovantePdfDados
{
    public int 			PedidoId 				{ get; set; }
    public string 		ProtocoloEstorno 		{ get; set; } = string.Empty;
    public string 		NomeComprador 			{ get; set; } = string.Empty;
    public string 		DocumentoComprador 		{ get; set; } = string.Empty;
    public string 		EmailComprador 			{ get; set; } = string.Empty;
    public string 		MotivoCodigo 			{ get; set; } = string.Empty;
    public string 		MotivoDescricao 		{ get; set; } = string.Empty;
    public string? 		MotivoInformado 		{ get; set; }
    public string? 		RegraAplicada 			{ get; set; }
    public string 		ValorEstornadoFormatado { get; set; } = string.Empty;
    public DateTime? 	DataSolicitacao 		{ get; set; }
    public DateTime? 	DataAprovacao 			{ get; set; }
    public DateTime? 	DataEstorno 			{ get; set; }
    public string 		EventoTitulo 			{ get; set; } = string.Empty;
    public DateTime? 	EventoData 				{ get; set; }
    public string? 		Setor 					{ get; set; }
    public int 			QuantidadeIngressos 	{ get; set; }
}
