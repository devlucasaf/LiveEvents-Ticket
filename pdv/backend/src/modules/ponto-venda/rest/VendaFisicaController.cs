using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Service;

namespace PontoVenda.Backend.Modules.PontoVenda.Rest;

[ApiController]
[Authorize]
[Route("api/pontovenda")]
public class VendaFisicaController : ControllerBase
{
    private readonly VendaFisicaService _service;

    // --- INJEÇÃO DO SERVICE DE VENDAS FÍSICAS ---
    public VendaFisicaController(VendaFisicaService service)
    {
        _service = service;
    }

    // --- REGISTRAR NOVA VENDA FÍSICA E GERAR TICKET ---
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] CriarVendaFisicaDto dto, CancellationToken cancellationToken)
    {
        var resposta = await _service.RegistrarAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(BuscarPorId), new { id = resposta.Id }, resposta);
    }

    // --- LISTAR TODAS AS VENDAS FÍSICAS ---
    [HttpGet("vendas")]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarAsync(cancellationToken));
    }

    // --- BUSCAR VENDA FÍSICA POR ID ---
    [HttpGet("vendas/{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id, CancellationToken cancellationToken)
    {
        var venda = await _service.BuscarPorIdAsync(id, cancellationToken);
        return venda is null ? NotFound() : Ok(venda);
    }

    // --- RESUMO AGREGADO POR EVENTO ---
    [HttpGet("relatorios/por-evento")]
    public async Task<IActionResult> RelatorioPorEvento(CancellationToken cancellationToken)
    {
        return Ok(await _service.ResumoPorEventoAsync(cancellationToken));
    }

    // --- VENDAS DETALHADAS DE UM EVENTO ---
    [HttpGet("relatorios/por-evento/{eventoId:guid}")]
    public async Task<IActionResult> RelatorioVendasDoEvento(Guid eventoId, CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarPorEventoAsync(eventoId, cancellationToken));
    }

    // --- RESUMO AGREGADO POR ATENDENTE ---
    [HttpGet("relatorios/por-atendente")]
    public async Task<IActionResult> RelatorioPorAtendente(CancellationToken cancellationToken)
    {
        return Ok(await _service.ResumoPorAtendenteAsync(cancellationToken));
    }

    // --- VENDAS DETALHADAS DE UM ATENDENTE ---
    [HttpGet("relatorios/por-atendente/{operadorId:int}")]
    public async Task<IActionResult> RelatorioVendasDoAtendente(int operadorId, CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarPorAtendenteAsync(operadorId, cancellationToken));
    }
}
