namespace PontoVenda.Backend.Modules.PontoVenda.Dto;

public class CriarVendaDto
{
    public List<ItemVendaDto>   Itens           { get; set; } = [];
    public string               FormaPagamento  { get; set; } = "DINHEIRO";
}
