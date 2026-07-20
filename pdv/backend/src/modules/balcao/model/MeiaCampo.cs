namespace PontoVenda.Backend.Modules.Balcao.Model;

public class MeiaCampo
{
    public string Chave       { get; set; }
    public string Rotulo      { get; set; }
    public string Tipo        { get; set; }
    public bool   Obrigatorio { get; set; }

    public MeiaCampo(string chave, string rotulo, string tipo = "text", bool obrigatorio = true)
    {
        Chave       = chave;
        Rotulo      = rotulo;
        Tipo        = tipo;
        Obrigatorio = obrigatorio;
    }
}
