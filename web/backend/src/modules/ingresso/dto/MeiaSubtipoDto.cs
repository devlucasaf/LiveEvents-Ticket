namespace LiveEventsTicket.Backend.Modules.Ingresso.Dto;

public class MeiaSubtipoDto
{
    public string                   Codigo { get; set; } = string.Empty;
    public string                   Nome   { get; set; } = string.Empty;
    public List<DocumentoCampoDto>  Campos { get; set; } = new();
}
