using PontoVenda.Backend.Entity;

namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class Produto : AuditEntity
{
    public int      Id              { get; set; }
    public string   Nome            { get; set; } = string.Empty;
    public string   CodigoBarras    { get; set; } = string.Empty;
    public decimal  Preco           { get; set; }
    public int      EstoqueAtual    { get; set; }
    public bool     Ativo           { get; set; } = true;
}
