using PagamentoEntity = LiveEventsTicket.Backend.Modules.Pagamento.Model.Pagamento;
using PedidoCheckinLogEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.PedidoCheckinLog;
using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Repository;

public interface IPedidoRepository
{
    Task AdicionarPedidoAsync(PedidoEntity pedido, CancellationToken cancellationToken = default);
    Task AdicionarPagamentoAsync(PagamentoEntity pagamento, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    Task<List<PedidoEntity>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task<List<PedidoEntity>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<PedidoEntity?> BuscarPorIdEUsuarioAsync(int pedidoId, int usuarioId, CancellationToken cancellationToken = default);
    Task<PedidoEntity?> BuscarPorCheckinTokenAsync(string checkinToken, CancellationToken cancellationToken = default);
    Task<PedidoEntity?> BuscarPorCompartilhamentoTokenAsync(string compartilhamentoToken, CancellationToken cancellationToken = default);
    Task<PagamentoEntity?> BuscarPagamentoPorPedidoIdAsync(int pedidoId, CancellationToken cancellationToken = default);
    Task AdicionarCheckinLogAsync(PedidoCheckinLogEntity log, CancellationToken cancellationToken = default);
}
