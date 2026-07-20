namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class VendaRelatorioBalcaoDto
{
    public Guid     Id              { get; set; }
    public string   CodigoTicket    { get; set; } = string.Empty;
    public DateTime DataVenda       { get; set; }
    public string   EventoNome      { get; set; } = string.Empty;
    public string   Setor           { get; set; } = string.Empty;
    public string   TipoEntrada     { get; set; } = string.Empty;
    public int      Quantidade      { get; set; }
    public string   OperadorNome    { get; set; } = string.Empty;
    public string   ClienteNome     { get; set; } = string.Empty;
    public string   MetodoPagamento { get; set; } = "Balcão";
    public decimal  Valor           { get; set; }
}
