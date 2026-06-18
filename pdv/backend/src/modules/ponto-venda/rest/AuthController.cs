using Microsoft.AspNetCore.Mvc;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Service;

namespace PontoVenda.Backend.Modules.PontoVenda.Rest;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly OperadorService _service;

    // --- INJEÇÃO DO SERVICE DE OPERADOR ---
    public AuthController(OperadorService service)
    {
        _service = service;
    }

    // --- LOGIN DO OPERADOR ---
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _service.LoginAsync(dto, cancellationToken));
    }
}
