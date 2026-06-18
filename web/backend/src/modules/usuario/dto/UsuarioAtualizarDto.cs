namespace LiveEventsTicket.Backend.Modules.Usuario.Dto;

public class UsuarioAtualizarDto
{
    public string?  Nome            { get; set; }
    public string?  Sobrenome       { get; set; }
    public string?  Email           { get; set; }
    public string?  Telefone        { get; set; }
    public string?  Cpf             { get; set; }
    public string?  SenhaAtual      { get; set; }
    public string?  NovaSenha       { get; set; }
}
