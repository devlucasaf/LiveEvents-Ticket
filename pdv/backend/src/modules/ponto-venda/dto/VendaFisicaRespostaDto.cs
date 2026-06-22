namespace PontoVenda.Backend.Modules.PontoVenda.Dto;

public class VendaFisicaRespostaDto
{
    public Guid     Id                  { get; set; }
    public string   CodigoTicket        { get; set; } = string.Empty;
    public Guid     EventoId            { get; set; }
    public string   EventoNome          { get; set; } = string.Empty;
    public string   EventoLocal         { get; set; } = string.Empty;
    public DateTime EventoData          { get; set; }
    public Guid     AssentoId           { get; set; }
    public string   AssentoSetor        { get; set; } = string.Empty;
    public string   AssentoFileira      { get; set; } = string.Empty;
    public int      AssentoNumero       { get; set; }
    public int      OperadorId          { get; set; }
    public string   OperadorNome        { get; set; } = string.Empty;
    public string   MetodoPagamento     { get; set; } = string.Empty;
    public decimal  Valor               { get; set; }
    public DateTime DataVenda           { get; set; }
}
