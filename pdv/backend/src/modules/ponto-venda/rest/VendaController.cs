using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoVenda.Backend.Infra.Security;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Service;

namespace PontoVenda.Backend.Modules.PontoVenda.Rest;

[ApiController]
[Authorize]
[Route("api/vendas")]
public class VendaController : ControllerBase
{
    private readonly VendaService _service;

    // --- INJEÇÃO DO SERVICE DE VENDAS ---
    public VendaController(VendaService service)
    {
        _service = service;
    }

    // --- REGISTRAR NOVA VENDA NO PDV ---
    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromServices] CurrentUserService currentUser,
        [FromBody] CriarVendaDto dto,
        CancellationToken cancellationToken)
    {
        var resposta = await _service.RegistrarAsync(currentUser.GetUserId(), dto, cancellationToken);
        return Ok(resposta);
    }

    // --- LISTAR TODAS AS VENDAS ---
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarAsync(cancellationToken));
    }
}
