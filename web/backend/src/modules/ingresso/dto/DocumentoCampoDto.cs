namespace LiveEventsTicket.Backend.Modules.Ingresso.Dto;

public class DocumentoCampoDto
{
    public string Chave       { get; set; } = string.Empty;
    public string Rotulo      { get; set; } = string.Empty;
    public string Tipo        { get; set; } = "text";
    public bool   Obrigatorio { get; set; } = true;
}
