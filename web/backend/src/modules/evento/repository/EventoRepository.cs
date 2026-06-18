using LiveEventsTicket.Backend.Infra.Config;
using Microsoft.EntityFrameworkCore;
using EventoEntity = LiveEventsTicket.Backend.Modules.Evento.Model.Evento;

namespace LiveEventsTicket.Backend.Modules.Evento.Repository;

public class EventoRepository : IEventoRepository
{
    private readonly AppDbContext _context;

    // --- INJEÇÃO DO CONTEXTO DO BANCO DE DADOS ---
    public EventoRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- LISTAR TODOS OS EVENTOS ORDENADOS POR DATA ---
    public Task<List<EventoEntity>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _context.Eventos.OrderBy(e => e.DataEvento).ToListAsync(cancellationToken);
    }

    // --- BUSCAR EVENTO POR ID ---
    public Task<EventoEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Eventos.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    // --- ADICIONAR NOVO EVENTO E SALVAR NO BANCO ---
    public async Task AdicionarAsync(EventoEntity evento, CancellationToken cancellationToken = default)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
