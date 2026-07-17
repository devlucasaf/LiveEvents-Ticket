namespace LiveEventsTicket.Backend.Modules.Ingresso.Model;

public static class CatalogoIngresso
{
    public const decimal MeiaFator = 0.5m;
    public const decimal SocialAcrescimo = 40m;

    // --- 5 SETORES OFICIAIS DISPONIVEIS EM TODO EVENTO ---
    public static readonly IReadOnlyList<SetorInfo> Setores = new List<SetorInfo>
    {
        new("PISTA",             "Pista",             150m),
        new("PISTA_PREMIUM",     "Pista Premium",     300m),
        new("CADEIRA_INFERIOR",  "Cadeira Inferior",  250m),
        new("CADEIRA_SUPERIOR",  "Cadeira Superior",  200m),
        new("ARQUIBANCADA",      "Arquibancada",      120m)
    };

    // --- SUBTIPOS ACEITOS DE MEIA ENTRADA ---
    public static readonly IReadOnlyList<MeiaSubtipoInfo> MeiaSubtipos = new List<MeiaSubtipoInfo>
    {
        new("ESTUDANTIL",          "Meia Estudantil"),
        new("PCD",                 "Meia PCD"),
        new("IDOSO",               "Meia Idoso"),
        new("PROFESSOR",           "Meia Professor"),
        new("MENOR_18",            "Meia Menor de 18 anos"),
        new("JOVEM_BAIXA_RENDA",   "Meia Jovem Baixa Renda"),
        new("DOADOR_SANGUE",       "Meia Doador de Sangue")
    };

    public const string ModalidadeInteira = "INTEIRA";
    public const string ModalidadeMeia    = "MEIA";
    public const string ModalidadeSocial  = "SOCIAL";

    // --- CALCULA O PRECO UNITARIO CONFORME A MODALIDADE ESCOLHIDA ---
    public static decimal CalcularPreco(decimal precoInteira, string? modalidade)
    {
        var mod = (modalidade ?? ModalidadeInteira).Trim().ToUpperInvariant();

        var meia = Math.Round(precoInteira * MeiaFator, 2);
        return mod switch
        {
            ModalidadeMeia   => meia,
            ModalidadeSocial => meia + SocialAcrescimo,
            _                => precoInteira
        };
    }

    // --- VERIFICA SE A MODALIDADE INFORMADA E VALIDA ---
    public static bool ModalidadeValida(string? modalidade)
    {
        var mod = (modalidade ?? string.Empty).Trim().ToUpperInvariant();
        return mod is ModalidadeInteira or ModalidadeMeia or ModalidadeSocial;
    }

    // --- VERIFICA SE O SUBTIPO DE MEIA INFORMADO E VALIDO ---
    public static bool SubtipoMeiaValido(string? subtipo)
    {
        var sub = (subtipo ?? string.Empty).Trim().ToUpperInvariant();
        return MeiaSubtipos.Any(s => s.Codigo == sub);
    }
}
