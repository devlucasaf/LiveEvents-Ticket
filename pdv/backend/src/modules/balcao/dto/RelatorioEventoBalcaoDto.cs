namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class RelatorioEventoBalcaoDto
{
    public int      EventoId         { get; set; }
    public string   EventoNome       { get; set; } = string.Empty;
    public string   EventoLocal      { get; set; } = string.Empty;
    public DateTime EventoData       { get; set; }
    public int      QuantidadeVendas { get; set; }
    public decimal  FaturamentoTotal { get; set; }
    public decimal  TicketMedio      { get; set; }
}
