using EventoEntity = LiveEventsTicket.Backend.Modules.Evento.Model.Evento;

namespace LiveEventsTicket.Backend.Modules.Evento.Repository;

public interface IEventoRepository
{
    Task<List<EventoEntity>> ListarAsync(CancellationToken cancellationToken = default);
    Task<EventoEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(EventoEntity evento, CancellationToken cancellationToken = default);
}
