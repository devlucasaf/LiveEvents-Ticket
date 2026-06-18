using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Service;

namespace PontoVenda.Backend.Modules.PontoVenda.Rest;

[ApiController]
[Authorize]
[Route("api/produtos")]
public class ProdutoController : ControllerBase
{
    private readonly ProdutoService _service;

    // --- INJEÇÃO DO SERVICE DE PRODUTOS ---
    public ProdutoController(ProdutoService service)
    {
        _service = service;
    }

    // --- LISTAR PRODUTOS ATIVOS ---
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarAsync(cancellationToken));
    }

    // --- BUSCAR PRODUTO POR CÓDIGO DE BARRAS ---
    [HttpGet("codigo-barras/{codigo}")]
    public async Task<IActionResult> BuscarPorCodigoBarras(string codigo, CancellationToken cancellationToken)
    {
        return Ok(await _service.BuscarPorCodigoBarrasAsync(codigo, cancellationToken));
    }

    // --- CRIAR NOVO PRODUTO (APENAS ADMIN) ---
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Criar([FromBody] ProdutoDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _service.CriarAsync(dto, cancellationToken));
    }
}
