using LiveEventsTicket.Backend.Infra.Config;
using Microsoft.EntityFrameworkCore;
using EventoEntity = LiveEventsTicket.Backend.Modules.Evento.Model.Evento;

namespace LiveEventsTicket.Backend.Modules.Evento.Repository;

public class EventoRepository : IEventoRepository
{
    private readonly AppDbContext _context;

    public EventoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<EventoEntity>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _context.Eventos.OrderBy(e => e.DataEvento).ToListAsync(cancellationToken);
    }

    public Task<EventoEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Eventos.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(EventoEntity evento, CancellationToken cancellationToken = default)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
