namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class RelatorioAtendenteBalcaoDto
{
    public int      OperadorId       { get; set; }
    public string   OperadorNome     { get; set; } = string.Empty;
    public string   OperadorLogin    { get; set; } = string.Empty;
    public int      QuantidadeVendas { get; set; }
    public decimal  FaturamentoTotal { get; set; }
    public decimal  TicketMedio      { get; set; }
}
