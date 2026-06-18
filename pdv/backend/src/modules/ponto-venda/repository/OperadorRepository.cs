using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public class OperadorRepository : IOperadorRepository
{
    private readonly AppDbContext _context;

    // --- INJEÇÃO DO CONTEXTO DO BANCO ---
    public OperadorRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- BUSCAR OPERADOR PELO LOGIN ---
    public Task<Operador?> BuscarPorLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return _context.Operadores.FirstOrDefaultAsync(o => o.Login == login, cancellationToken);
    }

    // --- BUSCAR OPERADOR POR ID ---
    public Task<Operador?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Operadores.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    // --- ADICIONAR NOVO OPERADOR ---
    public async Task AdicionarAsync(Operador operador, CancellationToken cancellationToken = default)
    {
        _context.Operadores.Add(operador);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
