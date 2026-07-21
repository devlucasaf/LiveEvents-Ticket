using LiveEventsTicket.Backend.Infra.Config;
using Microsoft.EntityFrameworkCore;
using PagamentoEntity = LiveEventsTicket.Backend.Modules.Pagamento.Model.Pagamento;
using PedidoCheckinLogEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.PedidoCheckinLog;
using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Repository;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    // --- INJEÇÃO DO CONTEXTO DO BANCO DE DADOS ---
    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- ADICIONAR NOVO PEDIDO E SALVAR NO BANCO ---
    public async Task AdicionarPedidoAsync(PedidoEntity pedido, CancellationToken cancellationToken = default)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- ADICIONAR PAGAMENTO VINCULADO AO PEDIDO ---
    public async Task AdicionarPagamentoAsync(PagamentoEntity pagamento, CancellationToken cancellationToken = default)
    {
        _context.Pagamentos.Add(pagamento);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- SALVAR ALTERAÇÕES PENDENTES NO BANCO ---
    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    // --- LISTAR PEDIDOS DE UM USUÁRIO COM ITENS ---
    public Task<List<PedidoEntity>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.UsuarioId == usuarioId)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    // --- LISTAR TODOS OS PEDIDOS COM ITENS ---
    public Task<List<PedidoEntity>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    // --- BUSCAR PEDIDO POR ID GARANTINDO O USUARIO DONO ---
    public Task<PedidoEntity?> BuscarPorIdEUsuarioAsync(int pedidoId, int usuarioId, CancellationToken cancellationToken = default)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == pedidoId && p.UsuarioId == usuarioId, cancellationToken);
    }

    // --- BUSCAR PEDIDO POR TOKEN DE CHECKIN ---
    public Task<PedidoEntity?> BuscarPorCheckinTokenAsync(string checkinToken, CancellationToken cancellationToken = default)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.CheckinToken == checkinToken, cancellationToken);
    }

    // --- BUSCAR PEDIDO POR TOKEN DE COMPARTILHAMENTO DE PDF ---
    public Task<PedidoEntity?> BuscarPorCompartilhamentoTokenAsync(string compartilhamentoToken, CancellationToken cancellationToken = default)
    {
        return _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.CompartilhamentoToken == compartilhamentoToken, cancellationToken);
    }

    // --- BUSCAR PAGAMENTO MAIS RECENTE DO PEDIDO ---
    public Task<PagamentoEntity?> BuscarPagamentoPorPedidoIdAsync(int pedidoId, CancellationToken cancellationToken = default)
    {
        return _context.Pagamentos
            .Where(pg => pg.PedidoId == pedidoId)
            .OrderByDescending(pg => pg.DataPagamento)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // --- REGISTRA CADA TENTATIVA DE CHECKIN COM RESULTADO ---
    public async Task AdicionarCheckinLogAsync(PedidoCheckinLogEntity log, CancellationToken cancellationToken = default)
    {
        _context.PedidoCheckinLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
