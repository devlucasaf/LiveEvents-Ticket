namespace LiveEventsTicket.Backend.Modules.Ingresso.Model;

public static class CatalogoIngresso
{
    public const decimal MeiaFator = 0.5m;
    public const decimal SocialAcrescimo = 20m;

    // --- CAMPOS DE DOCUMENTO COMUNS A TODO SUBTIPO DE MEIA ---
    private static readonly IReadOnlyList<DocumentoCampoInfo> CamposComuns = new List<DocumentoCampoInfo>
    {
        new("nome",           "Nome completo"),
        new("cpf",            "CPF"),
        new("dataNascimento", "Data de nascimento", "date")
    };

    // --- CAMPOS ESPECIFICOS EXIGIDOS POR CADA SUBTIPO DE MEIA ---
    private static readonly IReadOnlyDictionary<string, List<DocumentoCampoInfo>> CamposPorSubtipo =
        new Dictionary<string, List<DocumentoCampoInfo>>
        {
            ["ESTUDANTIL"] = new()
            {
                new("instituicaoEnsino",     "Instituição de ensino"),
                new("matricula",             "Matrícula"),
                new("dataPrevistaConclusao", "Data prevista de conclusão", "date"),
                new("curso",                 "Curso (se faculdade)", "text", false)
            },
            ["PCD"] = new()
            {
                new("cartaoBpc",  "Cartão BPC"),
                new("cidOuInss",  "CID ou nº do INSS")
            },
            ["PROFESSOR"] = new()
            {
                new("numeroCarteiraFuncional", "Número da carteira funcional"),
                new("matricula",               "Matrícula"),
                new("dataValidadeCarteira",    "Data de validade da carteira", "date")
            },
            ["JOVEM_BAIXA_RENDA"] = new()
            {
                new("carteiraIdentidadeJovem", "Carteira de identidade jovem"),
                new("dataValidadeCarteira",    "Data de validade da carteira", "date")
            },
            ["IDOSO"] = new()
            {
                new("numeroInss", "Número do INSS")
            },
            ["DOADOR_SANGUE"] = new()
            {
                new("idDoador", "ID do doador")
            },
            ["MENOR_18"] = new()
        };

    public const int IdadeMaximaMenor18 = 18;

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

    // --- RETORNA OS CAMPOS DE DOCUMENTO EXIGIDOS PARA UM SUBTIPO ---
    public static List<DocumentoCampoInfo> CamposDocumento(string? subtipo)
    {
        var sub = (subtipo ?? string.Empty).Trim().ToUpperInvariant();
        var campos = new List<DocumentoCampoInfo>(CamposComuns);
        if (CamposPorSubtipo.TryGetValue(sub, out var especificos))
        {
            campos.AddRange(especificos);
        }
        return campos;
    }

    // --- CALCULA A IDADE COMPLETA A PARTIR DA DATA DE NASCIMENTO ---
    public static int CalcularIdade(DateTime dataNascimento, DateTime? referencia = null)
    {
        var hoje = (referencia ?? DateTime.Today).Date;
        var idade = hoje.Year - dataNascimento.Year;
        if (dataNascimento.Date > hoje.AddYears(-idade))
        {
            idade--;
        }
        return idade;
    }

    // --- VALIDA OS DOCUMENTOS DE UMA MEIA ENTRADA ---
    public static void ValidarDocumentos(string subtipo, IReadOnlyDictionary<string, string?>? documentos)
    {
        var sub = (subtipo ?? string.Empty).Trim().ToUpperInvariant();
        var campos = CamposDocumento(sub);
        documentos ??= new Dictionary<string, string?>();

        foreach (var campo in campos.Where(c => c.Obrigatorio))
        {
            documentos.TryGetValue(campo.Chave, out var valor);
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new InvalidOperationException($"Informe o campo \"{campo.Rotulo}\" da meia entrada.");
            }
        }

        // --- REGRA ESPECIFICA DA MEIA MENOR DE 18 ANOS ---
        documentos.TryGetValue("dataNascimento", out var nascStr);
        if (DateTime.TryParse(nascStr, out var nascimento))
        {
            var idade = CalcularIdade(nascimento);
            if (sub == "MENOR_18" && idade >= IdadeMaximaMenor18)
            {
                throw new InvalidOperationException(
                    "Cliente com 18 anos ou mais não pode usar a meia Menor de 18. Selecione outra meia (ex.: Estudantil).");
            }
        }
    }
}
