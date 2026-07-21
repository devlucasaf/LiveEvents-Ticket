namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class PedidoRespostaDto
{
    public int      	Id              					{ get; set; }
    public decimal  	ValorTotal      					{ get; set; }
    public string   	Status          					{ get; set; } = string.Empty;
    public string?  	QrCodeBase64    					{ get; set; }
    public string   	PagamentoStatus 					{ get; set; } = string.Empty;
    public string?  	CodigoPix       					{ get; set; }
    public DateTime 	DataCriacao     					{ get; set; }
    public int?     	EventoId        					{ get; set; }
    public int?     	IngressoId      					{ get; set; }
    public string?  	Setor           					{ get; set; }
    public int      	Quantidade      					{ get; set; }
    public string?     	CompartilhamentoToken      			{ get; set; }
    public DateTime?   	CompartilhamentoExpiraEm   			{ get; set; }
    public DateTime?   	CompartilhamentoRevogadoEm 			{ get; set; }
    public int         	CompartilhamentoMaxAcessos 			{ get; set; }
    public int         	CompartilhamentoAcessosRealizados 	{ get; set; }
    public bool        	CompartilhamentoAtivo      			{ get; set; }
    public DateTime? 	ReembolsoSolicitadoEm  				{ get; set; }
    public DateTime? 	ReembolsoAprovadoEm    				{ get; set; }
    public DateTime? 	ReembolsoEstornadoEm   				{ get; set; }
    public string?   	ReembolsoMotivo        				{ get; set; }
    public string?   	ReembolsoMotivoCodigo  				{ get; set; }
    public string?   	ReembolsoRegraAplicada 				{ get; set; }
    public bool      	ReembolsoElegivel      				{ get; set; }
    public string?   	ReembolsoMensagem      				{ get; set; }
    public string?   	ReembolsoProtocoloEstorno 			{ get; set; }
}
