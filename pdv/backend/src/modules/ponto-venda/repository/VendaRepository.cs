using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public class VendaRepository : IVendaRepository
{
    private readonly AppDbContext _context;

    // --- INJEÇÃO DO CONTEXTO DO BANCO ---
    public VendaRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- ADICIONAR NOVA VENDA E SALVAR ---
    public async Task AdicionarAsync(Venda venda, CancellationToken cancellationToken = default)
    {
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- PERSISTIR ALTERAÇÕES PENDENTES ---
    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    // --- LISTAR TODAS AS VENDAS COM ITENS ---
    public Task<List<Venda>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _context.Vendas.Include(v => v.Itens).OrderByDescending(v => v.DataVenda).ToListAsync(cancellationToken);
    }

    // --- BUSCAR VENDA POR ID COM ITENS ---
    public Task<Venda?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Vendas.Include(v => v.Itens).FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }
}
