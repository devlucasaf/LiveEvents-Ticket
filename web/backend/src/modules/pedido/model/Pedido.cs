namespace LiveEventsTicket.Backend.Modules.Pedido.Model;

public class Pedido
{
    public int                  Id                  				{ get; set; }
    public int                  UsuarioId           				{ get; set; }
    public string               CompradorNome       				{ get; set; } = string.Empty;
    public string               CompradorCpf        				{ get; set; } = string.Empty;
    public string               CompradorEmail      				{ get; set; } = string.Empty;
    public string               CompradorTelefone       			{ get; set; } = string.Empty;
    public string               CompradorDataNascimento 			{ get; set; } = string.Empty;
    public string               EnderecoCep         				{ get; set; } = string.Empty;
    public string               EnderecoLogradouro  				{ get; set; } = string.Empty;
    public string               EnderecoNumero      				{ get; set; } = string.Empty;
    public string?              EnderecoComplemento 				{ get; set; }
    public string               EnderecoBairro      				{ get; set; } = string.Empty;
    public string               EnderecoCidade      				{ get; set; } = string.Empty;
    public string               EnderecoEstado      				{ get; set; } = string.Empty;
    public decimal              ValorTotal          				{ get; set; }
    public string               Status              				{ get; set; } = "CRIADO";
    public string?              QrCodeBase64        				{ get; set; }
    public string               CheckinToken            			{ get; set; } = string.Empty;
    public int                  CheckinUsosRealizados   			{ get; set; }
    public string?              CompartilhamentoToken   			{ get; set; }
    public DateTime?            CompartilhamentoExpiraEm 			{ get; set; }
    public DateTime?            CompartilhamentoRevogadoEm 			{ get; set; }
    public int                  CompartilhamentoMaxAcessos 			{ get; set; }
    public int                  CompartilhamentoAcessosRealizados 	{ get; set; }
    public DateTime?            ReembolsoSolicitadoEm   			{ get; set; }
    public DateTime?            ReembolsoAprovadoEm    				{ get; set; }
    public string?              ReembolsoMotivoCodigo   			{ get; set; }
    public string?              ReembolsoMotivo        				{ get; set; }
    public string?              ReembolsoRegraAplicada 				{ get; set; }
    public DateTime             DataCriacao         				{ get; set; } = DateTime.UtcNow;
    public List<ItemPedido>     Itens               				{ get; set; } = [];
}
