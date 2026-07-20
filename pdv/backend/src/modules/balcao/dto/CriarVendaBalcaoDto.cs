using System.ComponentModel.DataAnnotations;

namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class CriarVendaBalcaoDto
{
    [Required(ErrorMessage = "O evento é obrigatório.")]
    public int EventoId        { get; set; }

    [Required(ErrorMessage = "O tipo de ingresso é obrigatório.")]
    public int IngressoId      { get; set; }
    public string TipoEntrada     { get; set; } = "INTEIRA";
    public string? Subtipo        { get; set; }

    [Range(1, 20, ErrorMessage = "A quantidade deve ser entre 1 e 20.")]
    public int Quantidade      { get; set; } = 1;

    [Required(ErrorMessage = "A forma de pagamento é obrigatória.")]
    public string FormaPagamento { get; set; } = "CREDITO";

    [Required(ErrorMessage = "Os dados do cliente são obrigatórios.")]
    public ClienteBalcaoDto Cliente { get; set; } = new();
    public List<ClienteBalcaoDto> Acompanhantes { get; set; } = new();
}
