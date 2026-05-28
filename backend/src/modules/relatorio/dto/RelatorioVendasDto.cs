namespace LiveEventsTicket.Backend.Modules.Relatorio.Dto;

public class RelatorioVendasDto
{
    public int      TotalPedidos { get; set; }
    public decimal  ReceitaTotal { get; set; }
    public List<ResumoEventoDto> Eventos { get; set; } = [];
}
