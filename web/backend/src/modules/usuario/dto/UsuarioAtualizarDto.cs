using System.ComponentModel.DataAnnotations;

namespace LiveEventsTicket.Backend.Modules.Usuario.Dto;

public class UsuarioAtualizarDto
{
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string?  Nome            { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "O sobrenome deve ter entre 2 e 100 caracteres.")]
    public string?  Sobrenome       { get; set; }

    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    [StringLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres.")]
    public string?  Email           { get; set; }

    [StringLength(20, MinimumLength = 8, ErrorMessage = "Informe um telefone válido.")]
    public string?  Telefone        { get; set; }

    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "Informe um CPF válido.")]
    public string?  Cpf             { get; set; }

    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Informe um CEP válido.")]
    public string?  Cep             { get; set; }

    [StringLength(200)]
    public string?  Logradouro      { get; set; }

    [StringLength(20)]
    public string?  Numero          { get; set; }

    [StringLength(100)]
    public string?  Complemento     { get; set; }

    [StringLength(100)]
    public string?  Bairro          { get; set; }

    [StringLength(100)]
    public string?  Cidade          { get; set; }

    [RegularExpression(@"^[A-Za-z]{2}$", ErrorMessage = "Informe a sigla do estado com 2 letras.")]
    public string?  Estado          { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha atual deve ter entre 6 e 100 caracteres.")]
    public string?  SenhaAtual      { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova senha deve ter entre 6 e 100 caracteres.")]
    public string?  NovaSenha       { get; set; }
}
