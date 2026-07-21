namespace LiveEventsTicket.Backend.Modules.Usuario.Dto;

public class UsuarioRespostaDto
{
    public int      Id          { get; set; }
    public string   Nome        { get; set; } = string.Empty;
    public string   Sobrenome   { get; set; } = string.Empty;
    public string   Email       { get; set; } = string.Empty;
    public string   Telefone    { get; set; } = string.Empty;
    public string   Cpf         { get; set; } = string.Empty;
    public string   Cep         { get; set; } = string.Empty;
    public string   Logradouro  { get; set; } = string.Empty;
    public string   Numero      { get; set; } = string.Empty;
    public string?  Complemento { get; set; }
    public string   Bairro      { get; set; } = string.Empty;
    public string   Cidade      { get; set; } = string.Empty;
    public string   Estado      { get; set; } = string.Empty;
    public string   Role        { get; set; } = string.Empty;
}
