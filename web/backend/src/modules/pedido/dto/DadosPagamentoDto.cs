using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class DadosPagamentoDto
{
    [Required(ErrorMessage = "O tipo de pagamento é obrigatório.")]
    [RegularExpression(@"^(PIX|CARTAO)$", ErrorMessage = "Tipo de pagamento inválido. Utilize PIX ou CARTAO.")]
    public string   Tipo            { get; set; } = "PIX";

    [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Informe um número de cartão válido (13 a 19 dígitos).")]
    public string?  NumeroCartao    { get; set; }
}
