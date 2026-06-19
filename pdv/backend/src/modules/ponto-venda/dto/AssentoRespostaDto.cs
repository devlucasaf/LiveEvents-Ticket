namespace PontoVenda.Backend.Modules.PontoVenda.Dto;

public class AssentoRespostaDto
{
    public Guid     Id          { get; set; }
    public string   Setor       { get; set; } = string.Empty;
    public string   Fileira     { get; set; } = string.Empty;
    public int      Numero      { get; set; }
    public decimal  Preco       { get; set; }
    public string   Status      { get; set; } = string.Empty;
}
