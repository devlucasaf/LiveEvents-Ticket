using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoVenda.Backend.Infra.Security;
using PontoVenda.Backend.Modules.Balcao.Dto;
using PontoVenda.Backend.Modules.Balcao.Service;

namespace PontoVenda.Backend.Modules.Balcao.Rest;

[ApiController]
[Authorize]
[Route("api/balcao")]
public class BalcaoController : ControllerBase
{
    private readonly BalcaoService _service;
    private readonly CurrentUserService _currentUser;

    
    // --- GUARDA AS DEPENDENCIAS PARA USO NOS ENDPOINTS ---
    public BalcaoController(BalcaoService service, CurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // --- RETORNA OS EVENTOS DISPONIVEIS PARA VENDA ---
    [HttpGet("eventos")]
    public async Task<ActionResult<List<EventoBalcaoDto>>> ListarEventos(CancellationToken cancellationToken)
    {
        var eventos = await _service.ListarEventosAsync(cancellationToken);
        return Ok(eventos);
    }

    // --- RETORNA OS TIPOS DE INGRESSO (SETORES) DE UM EVENTO ---
    [HttpGet("eventos/{eventoId:int}/ingressos")]
    public async Task<ActionResult<List<IngressoBalcaoDto>>> ListarIngressos(int eventoId, CancellationToken cancellationToken)
    {
        var ingressos = await _service.ListarIngressosAsync(eventoId, cancellationToken);
        return Ok(ingressos);
    }

    // --- REGISTRA UMA NOVA VENDA DE BALCAO ---
    [HttpPost("vender")]
    public async Task<ActionResult<VendaBalcaoRespostaDto>> Vender([FromBody] CriarVendaBalcaoDto dto, CancellationToken cancellationToken)
    {
        var operadorId = _currentUser.GetUserId();
        var operadorNome = _currentUser.GetUserName();

        var resposta = await _service.RegistrarVendaAsync(dto, operadorId, operadorNome, cancellationToken);
        return Ok(resposta);
    }

    // --- LISTA TODAS AS VENDAS DE BALCAO PARA O RELATORIO ---
    [HttpGet("vendas")]
    public async Task<ActionResult<List<VendaRelatorioBalcaoDto>>> ListarVendas(CancellationToken cancellationToken)
    {
        var vendas = await _service.ListarVendasAsync(cancellationToken);
        return Ok(vendas);
    }

    // --- RESUMO DE VENDAS AGRUPADO POR EVENTO ---
    [HttpGet("relatorios/por-evento")]
    public async Task<ActionResult<List<RelatorioEventoBalcaoDto>>> RelatorioPorEvento(CancellationToken cancellationToken)
    {
        var resumo = await _service.ResumoPorEventoAsync(cancellationToken);
        return Ok(resumo);
    }

    // --- RESUMO DE VENDAS AGRUPADO POR ATENDENTE ---
    [HttpGet("relatorios/por-atendente")]
    public async Task<ActionResult<List<RelatorioAtendenteBalcaoDto>>> RelatorioPorAtendente(CancellationToken cancellationToken)
    {
        var resumo = await _service.ResumoPorAtendenteAsync(cancellationToken);
        return Ok(resumo);
    }
}
