using LiveEventsTicket.Backend.Modules.Evento.Dto;
using LiveEventsTicket.Backend.Modules.Evento.Repository;
using EventoEntity = LiveEventsTicket.Backend.Modules.Evento.Model.Evento;

namespace LiveEventsTicket.Backend.Modules.Evento.Service;

public class EventoService
{
    private readonly IEventoRepository _repository;

    // --- INJEÇÃO DE DEPENDÊNCIA DO REPOSITÓRIO ---
    public EventoService(IEventoRepository repository)
    {
        _repository = repository;
    }

    // --- LISTAR TODOS OS EVENTOS E MAPEAR PARA DTO ---
    public async Task<List<EventoResumoDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var eventos = await _repository.ListarAsync(cancellationToken);
        return eventos.Select(Map).ToList();
    }

    // --- BUSCAR EVENTO POR ID OU LANÇAR EXCEÇÃO ---
    public async Task<EventoResumoDto> BuscarAsync(int id, CancellationToken cancellationToken = default)
    {
        var evento = await _repository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Evento não encontrado.");

        return Map(evento);
    }

    // --- CRIAR NOVO EVENTO A PARTIR DO DTO ---
    public async Task<EventoResumoDto> CriarAsync(EventoCriarDto dto, CancellationToken cancellationToken = default)
    {
        var evento = new EventoEntity
        {
            Titulo = dto.Titulo,
            Categoria = dto.Categoria,
            Local = dto.Local,
            DataEvento = dto.DataEvento,
            Descricao = dto.Descricao,
            ImagemUrl = dto.ImagemUrl
        };

        await _repository.AdicionarAsync(evento, cancellationToken);
        return Map(evento);
    }

    // --- MAPEAMENTO DE ENTIDADE PARA DTO DE RESPOSTA ---
    private static EventoResumoDto Map(EventoEntity evento) => new()
    {
        Id = evento.Id,
        Titulo = evento.Titulo,
        Categoria = evento.Categoria,
        Local = evento.Local,
        DataEvento = evento.DataEvento,
        Descricao = evento.Descricao,
        ImagemUrl = evento.ImagemUrl
    };
}
