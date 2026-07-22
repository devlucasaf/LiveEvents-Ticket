using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class ItemPedidoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "IngressoId inválido.")]
    public int                                  IngressoId  { get; set; }

    [Range(1, 20, ErrorMessage = "A quantidade deve estar entre 1 e 20.")]
    public int                                  Quantidade  { get; set; }

    [Required(ErrorMessage = "A modalidade é obrigatória.")]
    [RegularExpression(@"^(INTEIRA|MEIA)$", ErrorMessage = "Modalidade inválida. Utilize INTEIRA ou MEIA.")]
    public string                               Modalidade  { get; set; } = "INTEIRA";

    [StringLength(50)]
    public string?                              SubtipoMeia { get; set; }

    public List<Dictionary<string, string?>>?   Documentos { get; set; }
}
