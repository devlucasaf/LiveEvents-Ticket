using LiveEventsTicket.Backend.Infra.Config;
using Microsoft.EntityFrameworkCore;
using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;

namespace LiveEventsTicket.Backend.Modules.Ingresso.Repository;

public class IngressoRepository : IIngressoRepository
{
    private readonly AppDbContext _context;

    public IngressoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<IngressoEntity>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken = default)
    {
        return _context.Ingressos
            .Where(i => i.EventoId == eventoId)
            .OrderBy(i => i.Setor)
            .ToListAsync(cancellationToken);
    }

    public Task<IngressoEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Ingressos.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public Task AtualizarAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
