using LiveEventsTicket.Backend.Infra.Security;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Pedido.Rest;

[ApiController]
[Authorize]
[Route("api/pedido")]
public class PedidoController : ControllerBase
{
    private readonly PedidoService _service;

    public PedidoController(PedidoService service)
    {
        _service = service;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(
        [FromServices] CurrentUserService currentUser,
        [FromBody] CriarPedidoDto dto,
        CancellationToken cancellationToken)
    {
        var resposta = await _service.CriarAsync(currentUser.GetUserId(), dto, cancellationToken);
        return Ok(resposta);
    }

    [HttpGet("meus")]
    public async Task<IActionResult> ListarMeus(
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarPorUsuarioAsync(currentUser.GetUserId(), cancellationToken));
    }
}
