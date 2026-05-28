using LiveEventsTicket.Backend.Infra.Config;
using Microsoft.EntityFrameworkCore;
using PagamentoEntity = LiveEventsTicket.Backend.Modules.Pagamento.Model.Pagamento;
using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Repository;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarPedidoAsync(PedidoEntity pedido, CancellationToken cancellationToken = default)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AdicionarPagamentoAsync(PagamentoEntity pagamento, CancellationToken cancellationToken = default)
    {
        _context.Pagamentos.Add(pagamento);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task<List<PedidoEntity>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.UsuarioId == usuarioId)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public Task<List<PedidoEntity>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }
}
