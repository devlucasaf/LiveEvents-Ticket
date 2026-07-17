using System.ComponentModel.DataAnnotations;

namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class CriarVendaBalcaoDto
{
    [Required(ErrorMessage = "O evento é obrigatório.")]
    public int      EventoId        { get; set; }

    [Required(ErrorMessage = "O tipo de ingresso é obrigatório.")]
    public int      IngressoId      { get; set; }

    // "INTEIRA" OU "MEIA"
    public string   TipoEntrada     { get; set; } = "INTEIRA";

    [Range(1, 20, ErrorMessage = "A quantidade deve ser entre 1 e 20.")]
    public int      Quantidade      { get; set; } = 1;

    [Required(ErrorMessage = "Os dados do cliente são obrigatórios.")]
    public ClienteBalcaoDto Cliente { get; set; } = new();
}
