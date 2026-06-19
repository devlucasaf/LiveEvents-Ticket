namespace PontoVenda.Backend.Modules.PontoVenda.Dto;

public class VendaFisicaRespostaDto
{
    public Guid     Id                  { get; set; }
    public Guid     EventoId            { get; set; }
    public Guid     AssentoId           { get; set; }
    public string   MetodoPagamento     { get; set; } = string.Empty;
    public decimal  Valor               { get; set; }
    public DateTime DataVenda           { get; set; }
}
