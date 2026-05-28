using LiveEventsTicket.Backend.Modules.Ingresso.Service;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Ingresso.Rest;

[ApiController]
[Route("api/ingresso")]
public class IngressoController : ControllerBase
{
    private readonly IngressoService _service;

    public IngressoController(IngressoService service)
    {
        _service = service;
    }

    [HttpGet("evento/{eventoId:int}")]
    public async Task<IActionResult> ListarPorEvento(int eventoId, CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarPorEventoAsync(eventoId, cancellationToken));
    }
}
