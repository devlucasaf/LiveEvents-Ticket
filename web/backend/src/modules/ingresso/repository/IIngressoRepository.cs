using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;

namespace LiveEventsTicket.Backend.Modules.Ingresso.Repository;

public interface IIngressoRepository
{
    Task<List<IngressoEntity>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken = default);
    Task<IngressoEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(IngressoEntity ingresso, CancellationToken cancellationToken = default);
    Task AdicionarVariosAsync(IEnumerable<IngressoEntity> ingressos, CancellationToken cancellationToken = default);
    Task AtualizarAsync(CancellationToken cancellationToken = default);
}
