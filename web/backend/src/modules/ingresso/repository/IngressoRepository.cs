using LiveEventsTicket.Backend.Infra.Config;
using Microsoft.EntityFrameworkCore;
using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;

namespace LiveEventsTicket.Backend.Modules.Ingresso.Repository;

public class IngressoRepository : IIngressoRepository
{
    private readonly AppDbContext _context;

    // --- INJEÇÃO DO CONTEXTO DO BANCO DE DADOS ---
    public IngressoRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- LISTAR INGRESSOS POR EVENTO ---
    public Task<List<IngressoEntity>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken = default)
    {
        return _context.Ingressos
            .Where(i => i.EventoId == eventoId)
            .OrderBy(i => i.Setor)
            .ToListAsync(cancellationToken);
    }

    // --- BUSCAR INGRESSO POR ID ---
    public Task<IngressoEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Ingressos.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    // --- ADICIONAR NOVO INGRESSO E SALVAR NO BANCO ---
    public async Task AdicionarAsync(IngressoEntity ingresso, CancellationToken cancellationToken = default)
    {
        _context.Ingressos.Add(ingresso);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- ADICIONAR VARIOS INGRESSOS DE UMA VEZ ---
    public async Task AdicionarVariosAsync(IEnumerable<IngressoEntity> ingressos, CancellationToken cancellationToken = default)
    {
        _context.Ingressos.AddRange(ingressos);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- SALVAR ALTERAÇÕES PENDENTES ---
    public Task AtualizarAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
