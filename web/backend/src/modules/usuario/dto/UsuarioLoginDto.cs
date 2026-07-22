using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Usuario.Dto;

public class UsuarioLoginDto
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [StringLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}
