using LiveEventsTicket.Backend.Modules.Admin.Dto;
using LiveEventsTicket.Backend.Modules.Admin.Service;
using LiveEventsTicket.Backend.Modules.Evento.Dto;
using LiveEventsTicket.Backend.Modules.Ingresso.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveEventsTicket.Backend.Modules.Admin.Rest;

[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _service;
    private readonly FuncionarioService _funcionarioService;

    // --- INJEÇÃO DE DEPENDÊNCIA DO ADMIN SERVICE ---
    public AdminController(AdminService service, FuncionarioService funcionarioService)
    {
        _service = service;
        _funcionarioService = funcionarioService;
    }

    // --- CRIAR NOVO EVENTO ---
    [HttpPost("evento")]
    public async Task<IActionResult> CriarEvento([FromBody] EventoCriarDto dto, CancellationToken cancellationToken)
    {
        var evento = await _service.CriarEventoAsync(dto, cancellationToken);
        return Ok(evento);
    }

    // --- CRIAR NOVO INGRESSO ---
    [HttpPost("ingresso")]
    public async Task<IActionResult> CriarIngresso([FromBody] IngressoCriarDto dto, CancellationToken cancellationToken)
    {
        var ingresso = await _service.CriarIngressoAsync(dto, cancellationToken);
        return Created($"api/ingresso/{ingresso.Id}", ingresso);
    }

    // --- OBTER RELATÓRIO DE VENDAS ---
    [HttpGet("relatorio/vendas")]
    public async Task<IActionResult> RelatorioVendas(CancellationToken cancellationToken)
    {
        return Ok(await _service.RelatorioVendasAsync(cancellationToken));
    }

    // --- LISTAR TODOS OS USUÁRIOS ---
    [HttpGet("usuarios")]
    public async Task<IActionResult> ListarUsuarios(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarUsuariosAsync(cancellationToken));
    }

    // --- ALTERAR ROLE DE UM USUÁRIO ---
    [HttpPut("usuarios/{id:int}/role")]
    public async Task<IActionResult> AlterarRole(int id, [FromBody] AlterarRoleDto dto, CancellationToken cancellationToken)
    {
        var usuario = await _service.AlterarRoleAsync(id, dto.Role, cancellationToken);
        return Ok(usuario);
    }

    // --- LISTAR FUNCIONÁRIOS DO PDV ---
    [HttpGet("funcionarios")]
    public async Task<IActionResult> ListarFuncionarios(CancellationToken cancellationToken)
    {
        return Ok(await _funcionarioService.ListarAsync(cancellationToken));
    }

    // --- CADASTRAR NOVO FUNCIONÁRIO ---
    [HttpPost("funcionarios")]
    public async Task<IActionResult> CriarFuncionario([FromBody] FuncionarioCriarDto dto, CancellationToken cancellationToken)
    {
        var funcionario = await _funcionarioService.CriarAsync(dto, cancellationToken);
        return Created($"api/admin/funcionarios/{funcionario.Id}", funcionario);
    }

    // --- EDITAR FUNCIONÁRIO ---
    [HttpPut("funcionarios/{id:int}")]
    public async Task<IActionResult> AtualizarFuncionario(int id, [FromBody] FuncionarioAtualizarDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _funcionarioService.AtualizarAsync(id, dto, cancellationToken));
    }

    // --- ATIVAR/DESATIVAR FUNCIONÁRIO ---
    [HttpPatch("funcionarios/{id:int}/status")]
    public async Task<IActionResult> AlterarStatusFuncionario(int id, [FromBody] FuncionarioStatusDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _funcionarioService.AlterarStatusAsync(id, dto.Ativo, cancellationToken));
    }
}
