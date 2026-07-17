namespace LiveEventsTicket.Backend.Modules.Ingresso.Model;

public class SetorInfo
{
    public string   Codigo      { get; set; }
    public string   Nome        { get; set; }
    public decimal  PrecoPadrao { get; set; }

    public SetorInfo(string codigo, string nome, decimal precoPadrao)
    {
        Codigo = codigo;
        Nome = nome;
        PrecoPadrao = precoPadrao;
    }
}
