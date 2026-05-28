using LiveEventsTicket.Backend.Modules.Evento.Dto;
using LiveEventsTicket.Backend.Modules.Evento.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Evento.Rest;

[ApiController]
[Route("api/evento")]
public class EventoController : ControllerBase
{
    private readonly EventoService _service;

    public EventoController(EventoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Buscar(int id, CancellationToken cancellationToken)
    {
        return Ok(await _service.BuscarAsync(id, cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Criar([FromBody] EventoCriarDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _service.CriarAsync(dto, cancellationToken));
    }
}
