using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Admin.Dto;

public class FuncionarioCriarDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string   Nome    { get; set; } = string.Empty;

    [Required(ErrorMessage = "O login é obrigatório.")]
    public string   Login   { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter ao menos 6 caracteres.")]
    public string   Senha   { get; set; } = string.Empty;

    public string   Role    { get; set; } = "OPERADOR";
}
