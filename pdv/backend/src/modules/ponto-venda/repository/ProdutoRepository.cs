using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    // --- INJEÇÃO DO CONTEXTO DO BANCO ---
    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- LISTAR TODOS OS PRODUTOS ATIVOS ---
    public Task<List<Produto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _context.Produtos.Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync(cancellationToken);
    }

    // --- BUSCAR PRODUTO POR ID ---
    public Task<Produto?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    // --- BUSCAR PRODUTO PELO CÓDIGO DE BARRAS ---
    public Task<Produto?> BuscarPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default)
    {
        return _context.Produtos.FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras, cancellationToken);
    }

    // --- ADICIONAR NOVO PRODUTO ---
    public async Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- PERSISTIR ALTERAÇÕES ---
    public Task AtualizarAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
