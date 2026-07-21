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

    // --- INJEÇÃO DE DEPENDÊNCIA DO PEDIDO SERVICE ---
    public PedidoController(PedidoService service)
    {
        _service = service;
    }

    // --- REALIZAR CHECKOUT / CRIAR PEDIDO ---
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(
        [FromServices] CurrentUserService currentUser,
        [FromBody] CriarPedidoDto dto,
        CancellationToken cancellationToken)
    {
        var resposta = await _service.CriarAsync(currentUser.GetUserId(), dto, cancellationToken);
        return Ok(resposta);
    }

    // --- LISTAR PEDIDOS DO USUÁRIO LOGADO ---
    [HttpGet("meus")]
    public async Task<IActionResult> ListarMeus(
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarPorUsuarioAsync(currentUser.GetUserId(), cancellationToken));
    }

    // --- SOLICITAR REEMBOLSO COM DECISAO AUTOMATICA ---
    [HttpPost("{pedidoId:int}/reembolso/solicitar")]
    public async Task<IActionResult> SolicitarReembolso(
        [FromServices] CurrentUserService currentUser,
        [FromRoute] int pedidoId,
        [FromBody] SolicitarReembolsoDto dto,
        CancellationToken cancellationToken)
    {
        var resposta = await _service.SolicitarReembolsoAsync(
            currentUser.GetUserId(),
            pedidoId,
            dto,
            cancellationToken);

        return Ok(resposta);
    }

    // --- BAIXAR COMPROVANTE DE ESTORNO EM PDF ---
    [HttpGet("{pedidoId:int}/reembolso/comprovante/pdf")]
    public async Task<IActionResult> BaixarComprovanteEstornoPdf(
        [FromServices] CurrentUserService currentUser,
        [FromRoute] int pedidoId,
        CancellationToken cancellationToken)
    {
        var (arquivo, nomeArquivo) = await _service.GerarComprovanteEstornoPdfAsync(
            currentUser.GetUserId(),
            pedidoId,
            cancellationToken);

        return File(arquivo, "application/pdf", nomeArquivo);
    }

    // --- BAIXAR O INGRESSO EM PDF DO PEDIDO ---
    [HttpGet("{pedidoId:int}/ingresso/pdf")]
    public async Task<IActionResult> BaixarIngressoPdf(
        [FromServices] CurrentUserService currentUser,
        [FromRoute] int pedidoId,
        CancellationToken cancellationToken)
    {
        var (arquivo, nomeArquivo) = await _service.GerarIngressoPdfAsync(
            currentUser.GetUserId(),
            pedidoId,
            cancellationToken);

        return File(arquivo, "application/pdf", nomeArquivo);
    }

    // --- GERAR LINK TEMPORARIO PARA COMPARTILHAR O PDF DO INGRESSO ---
    [HttpPost("{pedidoId:int}/compartilhar")]
    public async Task<IActionResult> GerarCompartilhamento(
        [FromServices] CurrentUserService currentUser,
        [FromRoute] int pedidoId,
        [FromBody] CriarCompartilhamentoIngressoDto? dto,
        CancellationToken cancellationToken)
    {
        var resposta = await _service.GerarCompartilhamentoIngressoAsync(
            currentUser.GetUserId(),
            pedidoId,
            dto ?? new CriarCompartilhamentoIngressoDto(),
            cancellationToken);

        resposta.UrlCompartilhamento = $"{Request.Scheme}://{Request.Host}/api/pedido/compartilhado/{resposta.Token}/pdf";
        return Ok(resposta);
    }

    // --- REVOGAR LINK COMPARTILHAVEL DE UM PEDIDO ---
    [HttpPost("{pedidoId:int}/compartilhar/revogar")]
    public async Task<IActionResult> RevogarCompartilhamento(
        [FromServices] CurrentUserService currentUser,
        [FromRoute] int pedidoId,
        CancellationToken cancellationToken)
    {
        var resposta = await _service.RevogarCompartilhamentoIngressoAsync(
            currentUser.GetUserId(),
            pedidoId,
            cancellationToken);

        resposta.UrlCompartilhamento = $"{Request.Scheme}://{Request.Host}/api/pedido/compartilhado/{resposta.Token}/pdf";
        return Ok(resposta);
    }

    // --- DOWNLOAD PUBLICO COM TOKEN TEMPORARIO E LIMITE DE ACESSOS ---
    [AllowAnonymous]
    [HttpGet("compartilhado/{token}/pdf")]
    public async Task<IActionResult> BaixarIngressoCompartilhadoPdf(
        [FromRoute] string token,
        CancellationToken cancellationToken)
    {
        var (arquivo, nomeArquivo) = await _service.BaixarIngressoPdfCompartilhadoAsync(token, cancellationToken);
        return File(arquivo, "application/pdf", nomeArquivo);
    }
}
