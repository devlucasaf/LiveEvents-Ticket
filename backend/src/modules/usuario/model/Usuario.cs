namespace LiveEventsTicket.Backend.Modules.Usuario.Model;

public class Usuario
{
    public int      Id           { get; set; }
    public string   Nome         { get; set; } = string.Empty;
    public string   Email        { get; set; } = string.Empty;
    public string   SenhaHash    { get; set; } = string.Empty;
    public string   Role         { get; set; } = "CLIENTE";
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
