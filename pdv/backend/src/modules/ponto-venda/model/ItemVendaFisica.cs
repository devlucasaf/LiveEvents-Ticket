namespace PontoVenda.Backend.Modules.PontoVenda.Model;

public class ItemVendaFisica
{
    public Guid     Id              { get; set; } = Guid.NewGuid();
    public Guid     VendaFisicaId   { get; set; }
    public Guid     AssentoId       { get; set; }
    public decimal  Preco           { get; set; }
}
