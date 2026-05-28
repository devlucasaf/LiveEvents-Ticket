namespace LiveEventsTicket.Backend.Modules.Usuario.Dto;

public class TokenRespostaDto
{
    public string Token { get; set; } = string.Empty;
    public UsuarioRespostaDto Usuario { get; set; } = new();
}
