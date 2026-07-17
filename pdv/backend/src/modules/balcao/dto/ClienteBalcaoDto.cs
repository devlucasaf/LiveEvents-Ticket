using System.ComponentModel.DataAnnotations;

namespace PontoVenda.Backend.Modules.Balcao.Dto;

public class ClienteBalcaoDto
{
    [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
    public string   Nome            { get; set; } = string.Empty;

    [Required(ErrorMessage = "O sobrenome do cliente é obrigatório.")]
    public string   Sobrenome       { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail do cliente é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string   Email           { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF do cliente é obrigatório.")]
    public string   Cpf             { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone do cliente é obrigatório.")]
    public string   Telefone        { get; set; } = string.Empty;

    public DateTime? DataNascimento { get; set; }

    // --- ENDERECO / RESIDENCIA ---
    public string?  Cep             { get; set; }
    public string?  Logradouro      { get; set; }
    public string?  Numero          { get; set; }
    public string?  Complemento     { get; set; }
    public string?  Bairro          { get; set; }
    public string?  Cidade          { get; set; }
    public string?  Estado          { get; set; }
}
