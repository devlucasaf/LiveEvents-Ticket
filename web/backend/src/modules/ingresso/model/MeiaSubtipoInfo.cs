namespace LiveEventsTicket.Backend.Modules.Ingresso.Model;

public class MeiaSubtipoInfo
{
    public string   Codigo  { get; set; }
    public string   Nome    { get; set; }

    public MeiaSubtipoInfo(string codigo, string nome)
    {
        Codigo = codigo;
        Nome = nome;
    }
}
