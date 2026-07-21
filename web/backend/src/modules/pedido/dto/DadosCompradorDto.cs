namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class DadosCompradorDto
{
    public string   Nome         	{ get; set; } = string.Empty;
    public string   Cpf          	{ get; set; } = string.Empty;
    public string   Email        	{ get; set; } = string.Empty;
    public string   Telefone        { get; set; } = string.Empty;
    public string   DataNascimento	{ get; set; } = string.Empty;
    public string   Cep          	{ get; set; } = string.Empty;
    public string   Logradouro   	{ get; set; } = string.Empty;
    public string   Numero       	{ get; set; } = string.Empty;
    public string?  Complemento  	{ get; set; }
    public string   Bairro       	{ get; set; } = string.Empty;
    public string   Cidade       	{ get; set; } = string.Empty;
    public string   Estado       	{ get; set; } = string.Empty;
}