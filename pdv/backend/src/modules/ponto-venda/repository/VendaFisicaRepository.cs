using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public class VendaFisicaRepository : IVendaFisicaRepository
{
    private readonly AppDbContext _context;

    // --- INJEÇÃO DO CONTEXTO DO BANCO ---
    public VendaFisicaRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- ADICIONAR NOVA VENDA FÍSICA E PERSISTIR ---
    public async Task AdicionarAsync(VendaFisica venda, CancellationToken cancellationToken = default)
    {
        _context.VendasFisicas.Add(venda);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- PERSISTIR ALTERAÇÕES PENDENTES ---
    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    // --- LISTAR TODAS AS VENDAS FÍSICAS ---
    public Task<List<VendaFisica>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _context.VendasFisicas
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(cancellationToken);
    }

    // --- LISTAR VENDAS DE UM EVENTO ESPECÍFICO ---
    public Task<List<VendaFisica>> ListarPorEventoAsync(Guid eventoId, CancellationToken cancellationToken = default)
    {
        return _context.VendasFisicas
            .Where(v => v.EventoId == eventoId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(cancellationToken);
    }

    // --- LISTAR VENDAS REGISTRADAS POR UM ATENDENTE ESPECÍFICO ---
    public Task<List<VendaFisica>> ListarPorOperadorAsync(int operadorId, CancellationToken cancellationToken = default)
    {
        return _context.VendasFisicas
            .Where(v => v.OperadorId == operadorId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(cancellationToken);
    }

    // --- BUSCAR VENDA FÍSICA POR ID ---
    public Task<VendaFisica?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.VendasFisicas
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    // --- VERIFICAR SE O ASSENTO JÁ FOI VENDIDO ---
    public Task<bool> AssentoJaVendidoAsync(Guid assentoId, CancellationToken cancellationToken = default)
    {
        return _context.VendasFisicas
            .AnyAsync(v => v.AssentoId == assentoId, cancellationToken);
    }
}
