namespace LiveEventsTicket.Backend.Modules.Usuario.Dto;

public class UsuarioRegistroDto
{
    public string   Nome                { get; set; } = string.Empty;
    public string   Sobrenome           { get; set; } = string.Empty;
    public string   Email               { get; set; } = string.Empty;
    public string   Cpf                 { get; set; } = string.Empty;
    public string   Telefone            { get; set; } = string.Empty;
    public DateTime DataNascimento      { get; set; }
    public string   Senha               { get; set; } = string.Empty;
}
