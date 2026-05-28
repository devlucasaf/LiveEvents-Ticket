using LiveEventsTicket.Backend.Modules.Ingresso.Dto;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;

namespace LiveEventsTicket.Backend.Modules.Ingresso.Service;

public class IngressoService
{
    private readonly IIngressoRepository _repository;

    public IngressoService(IIngressoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IngressoDisponivelDto>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken = default)
    {
        var ingressos = await _repository.ListarPorEventoAsync(eventoId, cancellationToken);
        return ingressos.Select(i => new IngressoDisponivelDto
        {
            Id = i.Id,
            EventoId = i.EventoId,
            Setor = i.Setor,
            Preco = i.Preco,
            QuantidadeDisponivel = i.QuantidadeDisponivel
        }).ToList();
    }
}
