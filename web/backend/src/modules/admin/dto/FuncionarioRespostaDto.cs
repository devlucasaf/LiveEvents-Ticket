namespace LiveEventsTicket.Backend.Modules.Admin.Dto;

public class FuncionarioRespostaDto
{
    public int      Id          { get; set; }
    public string   Nome        { get; set; } = string.Empty;
    public string   Login       { get; set; } = string.Empty;
    public string   Role        { get; set; } = "OPERADOR";
    public bool     Ativo       { get; set; }
    public DateTime CreatedAt   { get; set; }
}
