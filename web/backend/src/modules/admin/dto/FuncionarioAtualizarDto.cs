using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Admin.Dto;

public class FuncionarioAtualizarDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string   Nome    { get; set; } = string.Empty;

    public string   Role    { get; set; } = "OPERADOR";

    public string?  Senha   { get; set; }
}
