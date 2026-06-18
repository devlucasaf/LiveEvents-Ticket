namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class DadosPagamentoDto
{
    public string   Tipo            { get; set; } = "PIX";
    public string?  NumeroCartao    { get; set; }
}
