namespace LiveEventsTicket.Backend.Modules.Relatorio.Dto;

public class ResumoEventoDto
{
    public string   Evento                          { get; set; } = string.Empty;
    public int      QuantidadeIngressosVendidos     { get; set; }
    public decimal  Receita                         { get; set; }
}
