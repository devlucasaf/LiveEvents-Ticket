namespace LiveEventsTicket.Backend.Modules.Ingresso.Dto;

public class IngressoDisponivelDto
{
    public int      Id                      { get; set; }
    public int      EventoId                { get; set; }
    public string   Setor                   { get; set; } = string.Empty;
    public decimal  Preco                   { get; set; }
    public int      QuantidadeDisponivel    { get; set; }
}
