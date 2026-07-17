namespace PontoVenda.Backend.Modules.Balcao.Model;

public class EventoWeb
{
    public int      Id          { get; set; }
    public string   Titulo      { get; set; } = string.Empty;
    public string   Categoria   { get; set; } = string.Empty;
    public string   Local       { get; set; } = string.Empty;
    public DateTime DataEvento  { get; set; }
    public string   Descricao   { get; set; } = string.Empty;
    public string   ImagemUrl   { get; set; } = string.Empty;
}
