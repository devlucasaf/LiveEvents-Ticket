namespace PontoVenda.Backend.Modules.PontoVenda.Dto;

public class VendaRespostaDto
{
    public int      Id              { get; set; }
    public decimal  ValorTotal      { get; set; }
    public string   FormaPagamento  { get; set; } = string.Empty;
    public string   Status          { get; set; } = string.Empty;
    public DateTime DataVenda       { get; set; }
    public int      QuantidadeItens { get; set; }
}
