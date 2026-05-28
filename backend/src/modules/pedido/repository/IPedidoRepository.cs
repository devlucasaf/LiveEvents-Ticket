using PagamentoEntity = LiveEventsTicket.Backend.Modules.Pagamento.Model.Pagamento;
using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Repository;

public interface IPedidoRepository
{
    Task AdicionarPedidoAsync(PedidoEntity pedido, CancellationToken cancellationToken = default);
    Task AdicionarPagamentoAsync(PagamentoEntity pagamento, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    Task<List<PedidoEntity>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<List<PedidoEntity>> ListarTodosAsync(CancellationToken cancellationToken = default);
}
