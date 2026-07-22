using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class CriarPedidoDto
{
    // --- LISTA DE ITENS: PELO MENOS UM ITEM ---
    [Required(ErrorMessage = "A lista de itens é obrigatória.")]
    [MinLength(1, ErrorMessage = "O pedido deve conter ao menos um item.")]
    public List<ItemPedidoDto> Itens { get; set; } = [];

    // --- DADOS DO COMPRADOR: BLOCO OBRIGATORIO ---
    [Required(ErrorMessage = "Os dados do comprador são obrigatórios.")]
    public DadosCompradorDto Comprador { get; set; } = new();

    // --- DADOS DE PAGAMENTO: BLOCO OBRIGATORIO ---
    [Required(ErrorMessage = "Os dados de pagamento são obrigatórios.")]
    public DadosPagamentoDto Pagamento { get; set; } = new();
}
