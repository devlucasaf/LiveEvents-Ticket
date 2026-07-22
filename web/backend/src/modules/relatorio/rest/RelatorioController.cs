using LiveEventsTicket.Backend.Modules.Relatorio.Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Relatorio.Rest;

[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("api/relatorio")]
public class RelatorioController : ControllerBase
{
    private readonly RelatorioService _service;

    // --- RECEBE O SERVICO DE RELATORIO VIA INJECAO DE DEPENDENCIA ---
    public RelatorioController(RelatorioService service)
    {
        _service = service;
    }

    // --- RETORNA O RELATORIO DE VENDAS ---
    [HttpGet("vendas")]
    public async Task<IActionResult> Vendas(CancellationToken cancellationToken)
    {
        return Ok(await _service.GerarVendasAsync(cancellationToken));
    }
}
