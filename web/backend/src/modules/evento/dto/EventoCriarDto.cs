using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Evento.Dto;

public class EventoCriarDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 200 caracteres.")]
    public string   Titulo      { get; set; } = string.Empty;

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(100)]
    public string   Categoria   { get; set; } = string.Empty;

    [Required(ErrorMessage = "O local é obrigatório.")]
    [StringLength(200)]
    public string   Local       { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data do evento é obrigatória.")]
    public DateTime DataEvento  { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "A descrição deve ter entre 10 e 2000 caracteres.")]
    public string   Descricao   { get; set; } = string.Empty;

    [Required(ErrorMessage = "A URL da imagem é obrigatória.")]
    [Url(ErrorMessage = "Informe uma URL válida para a imagem.")]
    [StringLength(500)]
    public string   ImagemUrl   { get; set; } = string.Empty;
}
