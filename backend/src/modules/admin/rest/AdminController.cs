using LiveEventsTicket.Backend.Modules.Admin.Dto;
using LiveEventsTicket.Backend.Modules.Admin.Service;
using LiveEventsTicket.Backend.Modules.Evento.Dto;
using LiveEventsTicket.Backend.Modules.Ingresso.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Admin.Rest;

[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _service;

    public AdminController(AdminService service)
    {
        _service = service;
    }

    [HttpPost("evento")]
    public async Task<IActionResult> CriarEvento([FromBody] EventoCriarDto dto, CancellationToken cancellationToken)
    {
        var evento = await _service.CriarEventoAsync(dto, cancellationToken);
        return Ok(evento);
    }

    [HttpPost("ingresso")]
    public async Task<IActionResult> CriarIngresso([FromBody] IngressoCriarDto dto, CancellationToken cancellationToken)
    {
        var ingresso = await _service.CriarIngressoAsync(dto, cancellationToken);
        return Created($"api/ingresso/{ingresso.Id}", ingresso);
    }

    [HttpGet("relatorio/vendas")]
    public async Task<IActionResult> RelatorioVendas(CancellationToken cancellationToken)
    {
        return Ok(await _service.RelatorioVendasAsync(cancellationToken));
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> ListarUsuarios(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarUsuariosAsync(cancellationToken));
    }

    [HttpPut("usuarios/{id:int}/role")]
    public async Task<IActionResult> AlterarRole(int id, [FromBody] AlterarRoleDto dto, CancellationToken cancellationToken)
    {
        var usuario = await _service.AlterarRoleAsync(id, dto.Role, cancellationToken);
        return Ok(usuario);
    }
}
