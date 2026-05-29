using LiveEventsTicket.Backend.Modules.Ingresso.Dto;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;

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

    public async Task<IngressoDisponivelDto> CriarAsync(IngressoCriarDto dto, CancellationToken cancellationToken = default)
    {
        var ingresso = new IngressoEntity
        {
            EventoId = dto.EventoId,
            Setor = dto.Setor,
            Preco = dto.Preco,
            QuantidadeDisponivel = dto.QuantidadeDisponivel
        };

        await _repository.AdicionarAsync(ingresso, cancellationToken);

        return new IngressoDisponivelDto
        {
            Id = ingresso.Id,
            EventoId = ingresso.EventoId,
            Setor = ingresso.Setor,
            Preco = ingresso.Preco,
            QuantidadeDisponivel = ingresso.QuantidadeDisponivel
        };
    }
}
