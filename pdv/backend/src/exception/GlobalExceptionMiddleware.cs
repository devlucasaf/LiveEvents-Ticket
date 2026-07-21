using System.Net;
using System.Text.Json;

namespace PontoVenda.Backend.Exception;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _proximo;

    // --- INJEÇÃO DO PRÓXIMO MIDDLEWARE DO PIPELINE ---
    public GlobalExceptionMiddleware(RequestDelegate proximo)
    {
        _proximo = proximo;
    }

    // --- INTERCEPTAR REQUISIÇÕES E CAPTURAR EXCEÇÕES NÃO TRATADAS ---
    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (System.Exception excecao)
        {
            await TratarExcecaoAsync(contexto, excecao);
        }
    }

    // --- MAPEAR EXCEÇÃO PARA RESPOSTA HTTP ESTRUTURADA ---
    private static Task TratarExcecaoAsync(HttpContext contexto, System.Exception excecao)
    {
        var (codigoStatus, mensagem) = excecao switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, excecao.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, excecao.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, excecao.Message),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor.")
        };

        contexto.Response.ContentType = "application/json";
        contexto.Response.StatusCode = (int)codigoStatus;

        var conteudo = JsonSerializer.Serialize(new ErrorResponse
        {
            Message = mensagem,
            StatusCode = contexto.Response.StatusCode
        });

        return contexto.Response.WriteAsync(conteudo);
    }
}
