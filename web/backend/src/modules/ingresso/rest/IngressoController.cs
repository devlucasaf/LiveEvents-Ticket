using LiveEventsTicket.Backend.Modules.Ingresso.Dto;
using LiveEventsTicket.Backend.Modules.Ingresso.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Ingresso.Rest;

[ApiController]
[Route("api/ingresso")]
public class IngressoController : ControllerBase
{
    private readonly IngressoService _service;

    // --- INJEÇÃO DE DEPENDÊNCIA DO INGRESSO SERVICE ---
    public IngressoController(IngressoService service)
    {
        _service = service;
    }

    // --- LISTAR INGRESSOS POR EVENTO ---
    [HttpGet("evento/{eventoId:int}")]
    public async Task<IActionResult> ListarPorEvento(int eventoId, CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarPorEventoAsync(eventoId, cancellationToken));
    }

    // --- LISTAR MODALIDADES DISPONIVEIS ---
    [HttpGet("modalidades")]
    public IActionResult ListarModalidades()
    {
        return Ok(_service.ObterModalidades());
    }

    // --- CRIAR NOVO INGRESSO ---
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Criar([FromBody] IngressoCriarDto dto, CancellationToken cancellationToken)
    {
        var resultado = await _service.CriarAsync(dto, cancellationToken);
        return Created($"api/ingresso/{resultado.Id}", resultado);
    }
}
