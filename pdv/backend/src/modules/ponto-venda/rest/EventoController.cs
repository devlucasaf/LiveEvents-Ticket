using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Modules.PontoVenda.Dto;

namespace PontoVenda.Backend.Modules.PontoVenda.Rest;

[ApiController]
[Authorize]
[Route("api/eventos")]
public class EventoController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventoController(AppDbContext context)
    {
        _context = context;
    }

    // --- LISTAR EVENTOS ATIVOS ---
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var eventos = await _context.Eventos
            .Where(e => e.Ativo)
            .OrderBy(e => e.DataEvento)
            .Select(e => new EventoRespostaDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Local = e.Local,
                DataEvento = e.DataEvento,
                Ativo = e.Ativo
            })
            .ToListAsync(cancellationToken);

        return Ok(eventos);
    }

    // --- LISTAR ASSENTOS DE UM EVENTO ---
    [HttpGet("{id:guid}/assentos")]
    public async Task<IActionResult> ListarAssentos(Guid id, [FromQuery] string? status, CancellationToken cancellationToken)
    {
        var query = _context.Assentos.Where(a => a.EventoId == id);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status.ToUpper());
        }

        var assentos = await query
            .OrderBy(a => a.Setor)
            .ThenBy(a => a.Fileira)
            .ThenBy(a => a.Numero)
            .Select(a => new AssentoRespostaDto
            {
                Id = a.Id,
                Setor = a.Setor,
                Fileira = a.Fileira,
                Numero = a.Numero,
                Preco = a.Preco,
                Status = a.Status
            })
            .ToListAsync(cancellationToken);

        return Ok(assentos);
    }
}
