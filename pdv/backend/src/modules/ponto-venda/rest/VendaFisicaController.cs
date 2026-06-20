using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Service;

namespace PontoVenda.Backend.Modules.PontoVenda.Rest;

[ApiController]
[Authorize]
[Route("api/vendas-fisicas")]
public class VendaFisicaController : ControllerBase
{
    private readonly VendaFisicaService _service;

    // --- INJEÇÃO DO SERVICE DE VENDAS FÍSICAS ---
    public VendaFisicaController(VendaFisicaService service)
    {
        _service = service;
    }

    // --- REGISTRAR NOVA VENDA FÍSICA ---
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] CriarVendaFisicaDto dto, CancellationToken cancellationToken)
    {
        var resposta = await _service.RegistrarAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(BuscarPorId), new { id = resposta.Id }, resposta);
    }

    // --- LISTAR TODAS AS VENDAS FÍSICAS ---
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarAsync(cancellationToken));
    }

    // --- BUSCAR VENDA FÍSICA POR ID ---
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id, CancellationToken cancellationToken)
    {
        var vendas = await _service.ListarAsync(cancellationToken);
        var venda = vendas.FirstOrDefault(v => v.Id == id);
        return venda is null ? NotFound() : Ok(venda);
    }
}
