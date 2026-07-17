namespace PontoVenda.Backend.Modules.Balcao.Model;

public class ClienteWeb
{
    public int      Id              { get; set; }
    public string   Nome            { get; set; } = string.Empty;
    public string   Sobrenome       { get; set; } = string.Empty;
    public string   Email           { get; set; } = string.Empty;
    public string   Cpf             { get; set; } = string.Empty;
    public string   Telefone        { get; set; } = string.Empty;
    public string   SenhaHash       { get; set; } = string.Empty;
    public string   Role            { get; set; } = "CLIENTE";
    public DateTime DataCadastro    { get; set; } = DateTime.UtcNow;
    public DateTime DataNascimento  { get; set; } = DateTime.UtcNow;
}
