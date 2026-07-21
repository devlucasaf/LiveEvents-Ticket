namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

internal sealed class MotivoReembolsoResolvido
{
    public MotivoReembolsoResolvido(
        string codigo,
        string descricao,
        string? detalhe,
        string motivoPersistido)
    {
        Codigo = codigo;
        Descricao = descricao;
        Detalhe = detalhe;
        MotivoPersistido = motivoPersistido;
    }

    public string Codigo { get; }
    public string Descricao { get; }
    public string? Detalhe { get; }
    public string MotivoPersistido { get; }
}
