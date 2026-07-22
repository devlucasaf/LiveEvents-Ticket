using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Pedido.Dto;

public class DadosCompradorDto
{
    [Required(ErrorMessage = "O nome do comprador é obrigatório.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 200 caracteres.")]
    public string   Nome         	{ get; set; } = string.Empty;

    // --- CPF: 11 DIGITOS COM OU SEM MASCARA ---
    [Required(ErrorMessage = "O CPF do comprador é obrigatório.")]
    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Informe um CPF válido.")]
    public string   Cpf          	{ get; set; } = string.Empty;

    [Required(ErrorMessage = "O email do comprador é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [StringLength(200)]
    public string   Email        	{ get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone do comprador é obrigatório.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Informe um telefone válido.")]
    public string   Telefone        { get; set; } = string.Empty;

    // --- DATA DE NASCIMENTO EM STRING (MANTIDO O CONTRATO EXISTENTE COM O FRONT) ---
    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    [StringLength(20)]
    public string   DataNascimento	{ get; set; } = string.Empty;

    // --- CEP: 8 DIGITOS COM OU SEM MASCARA ---
    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Informe um CEP válido.")]
    public string   Cep          	{ get; set; } = string.Empty;

    [Required(ErrorMessage = "O logradouro é obrigatório.")]
    [StringLength(200)]
    public string   Logradouro   	{ get; set; } = string.Empty;

    [Required(ErrorMessage = "O número é obrigatório.")]
    [StringLength(20)]
    public string   Numero       	{ get; set; } = string.Empty;

    [StringLength(100)]
    public string?  Complemento  	{ get; set; }

    [Required(ErrorMessage = "O bairro é obrigatório.")]
    [StringLength(100)]
    public string   Bairro       	{ get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(100)]
    public string   Cidade       	{ get; set; } = string.Empty;

    // --- ESTADO: SIGLA DE 2 LETRAS ---
    [Required(ErrorMessage = "O estado é obrigatório.")]
    [RegularExpression(@"^[A-Za-z]{2}$", ErrorMessage = "Informe a sigla do estado com 2 letras.")]
    public string   Estado       	{ get; set; } = string.Empty;
}