using LiveEventsTicket.Backend.Infra.Security;
using LiveEventsTicket.Backend.Modules.Usuario.Dto;
using LiveEventsTicket.Backend.Modules.Usuario.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Usuario.Rest;

[ApiController]
[Route("api/usuario")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _service;

    public UsuarioController(UsuarioService service)
    {
        _service = service;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] UsuarioRegistroDto dto, CancellationToken cancellationToken)
    {
        var usuario = await _service.RegistrarAsync(dto, cancellationToken);
        return Ok(usuario);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto, CancellationToken cancellationToken)
    {
        var token = await _service.LoginAsync(dto, cancellationToken);
        return Ok(token);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        var usuario = await _service.BuscarPorIdAsync(currentUser.GetUserId(), cancellationToken);
        return Ok(usuario);
    }
}
