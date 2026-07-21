namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class IngressoPdfDados
{
    public int 						PedidoId 			{ get; set; }
    public string 					NomeComprador 		{ get; set; } = string.Empty;
    public string 					DocumentoComprador 	{ get; set; } = string.Empty;
    public string 					EmailComprador 		{ get; set; } = string.Empty;
    public string 					EnderecoComprador 	{ get; set; } = string.Empty;
    public string 					DataCompra 			{ get; set; } = string.Empty;
    public string 					ValorTotalFormatado { get; set; } = string.Empty;
    public string? 					QrCodeBase64 		{ get; set; }
    public List<IngressoPdfLinha> 	Itens 				{ get; set; } = [];
}
