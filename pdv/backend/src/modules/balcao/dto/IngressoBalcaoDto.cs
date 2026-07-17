namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class IngressoBalcaoDto
{
    public int      Id                      { get; set; }
    public string   Setor                   { get; set; } = string.Empty;
    public decimal  Preco                   { get; set; }
    public int      QuantidadeDisponivel    { get; set; }
}
