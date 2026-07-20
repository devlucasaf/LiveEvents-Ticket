namespace PontoVenda.Backend.Modules.Balcao.Model;

public static class CatalogoMeia
{
    public const decimal MeiaFator = 0.5m;
    public const decimal SocialAcrescimo = 20m;
    public const int IdadeMaximaMenor18 = 18;

    // --- SUBTIPOS ACEITOS ---
    public static readonly IReadOnlyDictionary<string, string> Subtipos = new Dictionary<string, string>
    {
        ["ESTUDANTIL"]        = "Meia Estudantil",
        ["PCD"]               = "Meia PCD",
        ["IDOSO"]             = "Meia Idoso",
        ["PROFESSOR"]         = "Meia Professor",
        ["MENOR_18"]          = "Meia Menor de 18 anos",
        ["JOVEM_BAIXA_RENDA"] = "Meia Jovem Baixa Renda",
        ["DOADOR_SANGUE"]     = "Meia Doador de Sangue"
    };

    // --- CAMPOS EXTRAS ---
    private static readonly IReadOnlyDictionary<string, List<MeiaCampo>> CamposPorSubtipo =
        new Dictionary<string, List<MeiaCampo>>
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
                new("cartaoBpc", "Cartão BPC"),
                new("cidOuInss", "CID ou nº do INSS")
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

    // --- VERIFICA SE O SUBTIPO INFORMADO E VALIDO ---
    public static bool SubtipoValido(string? subtipo)
    {
        var sub = (subtipo ?? string.Empty).Trim().ToUpperInvariant();
        return Subtipos.ContainsKey(sub);
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

    // --- VALIDA OS DOCUMENTOS EXTRAS + REGRA DE IDADE DE UMA PESSOA ---
    public static void ValidarPessoa(string subtipo, IReadOnlyDictionary<string, string?>? documentos, DateTime? dataNascimento, string quem)
    {
        var sub = (subtipo ?? string.Empty).Trim().ToUpperInvariant();
        documentos ??= new Dictionary<string, string?>();

        if (CamposPorSubtipo.TryGetValue(sub, out var campos))
        {
            foreach (var campo in campos.Where(c => c.Obrigatorio))
            {
                documentos.TryGetValue(campo.Chave, out var valor);
                if (string.IsNullOrWhiteSpace(valor))
                {
                    throw new InvalidOperationException($"{quem}: informe o campo \"{campo.Rotulo}\" da meia entrada.");
                }
            }
        }

        // --- REGRA DA MEIA MENOR DE 18 ANOS ---
        if (sub == "MENOR_18" && dataNascimento.HasValue)
        {
            var idade = CalcularIdade(dataNascimento.Value);
            if (idade >= IdadeMaximaMenor18)
            {
                throw new InvalidOperationException(
                    $"{quem}: cliente com 18 anos ou mais não pode usar a meia Menor de 18. Selecione outra meia (ex.: Estudantil).");
            }
        }
    }
}
