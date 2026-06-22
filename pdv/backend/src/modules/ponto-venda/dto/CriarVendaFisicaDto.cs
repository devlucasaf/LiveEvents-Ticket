using System.ComponentModel.DataAnnotations;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Dto;

public class CriarVendaFisicaDto
{
    [Required(ErrorMessage = "O EventoId é obrigatório.")]
    public Guid EventoId { get; set; }

    [Required(ErrorMessage = "O AssentoId é obrigatório.")]
    public Guid AssentoId { get; set; }

    [Required(ErrorMessage = "O método de pagamento é obrigatório.")]
    [EnumDataType(typeof(MetodoPagamento), ErrorMessage = "Método de pagamento inválido. Valores aceitos: Dinheiro, Cartao, Pix.")]
    public MetodoPagamento MetodoPagamento { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Range(0.01, 99999.99, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}
