using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Pagamento.Rest;

[ApiController]
[Authorize]
[Route("api/pagamento")]
public class PagamentoController : ControllerBase
{
    // --- VERIFICA SE O MÓDULO ESTÁ ATIVO ---
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "ok", 
            message = "Módulo de pagamento ativo." 
        });
    }
}
