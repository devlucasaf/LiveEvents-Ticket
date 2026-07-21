using LiveEventsTicket.Backend.Infra.Security;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Pedido.Rest;

[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("api/checkin")]
public class CheckinController : ControllerBase
{
    private readonly PedidoService _pedidoService;

    public CheckinController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    // --- VALIDA TOKEN DO QR E REGISTRA O CHECKIN EM TEMPO REAL ---
    [HttpPost("validar")]
    public async Task<IActionResult> Validar(
        [FromServices] CurrentUserService currentUser,
        [FromBody] ValidarCheckinDto dto,
        CancellationToken cancellationToken)
    {
        var resposta = await _pedidoService.ValidarCheckinAsync(currentUser.GetUserId(), dto, cancellationToken);
        return Ok(resposta);
    }
}
