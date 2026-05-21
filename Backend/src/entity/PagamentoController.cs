using Backend.src.dto;
using Backend.src.entity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.src.modules.pagamento;

[ApiController]
[Route("api/[controller]")]
public class PagamentoController : ControllerBase
{
    [HttpPost("checkout")]
    public ActionResult<Pagamento> Checkout([FromBody] PagamentoDto dto)
    {
        var pagamento = new Pagamento
        {
            PedidoId    = dto.PedidoId,
            Metodo      = dto.Metodo,
            CriadoEm    = DateTime.UtcNow
        };

        if (dto.Metodo == "Cartao")
        {
            bool aprovado = !string.IsNullOrEmpty(dto.NumeroCartao) && 
                            char.IsDigit(dto.NumeroCartao.Last()) && 
                            (dto.NumeroCartao.Last() - '0') % 2 == 0;

            pagamento.Status = aprovado ? "Aprovado" : "Recusado";
            pagamento.NumeroCartao = dto.NumeroCartao?.Length > 4 
                ? $"**** **** **** {dto.NumeroCartao[^4..]}" 
                : dto.NumeroCartao;
                
            if (aprovado) 
            {
                pagamento.ConfirmadoEm = DateTime.UtcNow;
            }
        }
        else if (dto.Metodo == "Pix")
        {
            pagamento.CodigoPix = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20).ToUpper();
            pagamento.Status = "Aprovado";
            pagamento.ConfirmadoEm = DateTime.UtcNow;
        }
        else
        {
            pagamento.Status = "Recusado";
            return BadRequest(new { mensagem = "Método de pagamento inválido." });
        }

        return Ok(pagamento);
    }
}