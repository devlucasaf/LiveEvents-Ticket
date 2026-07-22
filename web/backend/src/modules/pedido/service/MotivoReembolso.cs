namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public static class MotivoReembolso
{
    public const string Arrependimento    = "ARREPENDIMENTO_7_DIAS";
    public const string Impedimento       = "IMPEDIMENTO_PESSOAL";
    public const string AlteracaoPlanos   = "ALTERACAO_DE_PLANOS";
    public const string ErroCompra        = "ERRO_NA_COMPRA";
    public const string Outro             = "OUTRO";

    // --- DESCRICAO PADRAO EXIBIDA AO USUARIO PARA CADA CODIGO ---
    public static readonly IReadOnlyDictionary<string, string> Descricoes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { Arrependimento,   "Arrependimento dentro do prazo legal de 7 dias." },
        { Impedimento,      "Impedimento pessoal para comparecer ao evento." },
        { AlteracaoPlanos,  "Mudança de planos do comprador." },
        { ErroCompra,       "Erro na compra (setor, quantidade ou dados)." },
        { Outro,            "Outro motivo informado pelo comprador." }
    };
}
