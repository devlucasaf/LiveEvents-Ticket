namespace Backend.src.dto;

public class PagamentoDto
{
    public int      PedidoId        { get; set; }
    public string   Metodo          { get; set; } = string.Empty;
    public string?  NumeroCartao    { get; set; }
    public string?  NomeTitular     { get; set; }
    public string?  Validade        { get; set; }
    public string?  Cvv             { get; set; }
}