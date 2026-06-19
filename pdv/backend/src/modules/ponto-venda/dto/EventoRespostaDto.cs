namespace PontoVenda.Backend.Modules.PontoVenda.Dto;

public class EventoRespostaDto
{
    public Guid     Id          { get; set; }
    public string   Nome        { get; set; } = string.Empty;
    public string   Local       { get; set; } = string.Empty;
    public DateTime DataEvento  { get; set; }
    public bool     Ativo       { get; set; }
}
