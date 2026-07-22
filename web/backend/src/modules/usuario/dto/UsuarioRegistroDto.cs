using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Usuario.Dto;

public class UsuarioRegistroDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string   Nome                { get; set; } = string.Empty;

    [Required(ErrorMessage = "O sobrenome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O sobrenome deve ter entre 2 e 100 caracteres.")]
    public string   Sobrenome           { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [StringLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres.")]
    public string   Email               { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Informe um CPF válido.")]
    public string   Cpf                 { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Informe um telefone válido.")]
    public string   Telefone            { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime DataNascimento      { get; set; }

    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Informe um CEP válido.")]
    public string   Cep                 { get; set; } = string.Empty;

    [Required(ErrorMessage = "O logradouro é obrigatório.")]
    [StringLength(200)]
    public string   Logradouro          { get; set; } = string.Empty;

    [Required(ErrorMessage = "O número é obrigatório.")]
    [StringLength(20)]
    public string   Numero              { get; set; } = string.Empty;

    [StringLength(100)]
    public string?  Complemento         { get; set; }

    [Required(ErrorMessage = "O bairro é obrigatório.")]
    [StringLength(100)]
    public string   Bairro              { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(100)]
    public string   Cidade              { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado é obrigatório.")]
    [RegularExpression(@"^[A-Za-z]{2}$", ErrorMessage = "Informe a sigla do estado com 2 letras.")]
    public string   Estado              { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
    public string   Senha               { get; set; } = string.Empty;
}
