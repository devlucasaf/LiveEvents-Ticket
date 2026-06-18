using System.Net;
using System.Text.Json;

namespace PontoVenda.Backend.Exception;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    // --- INJEÇÃO DO PRÓXIMO MIDDLEWARE DO PIPELINE ---
    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // --- INTERCEPTAR REQUISIÇÕES E CAPTURAR EXCEÇÕES NÃO TRATADAS ---
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (System.Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    // --- MAPEAR EXCEÇÃO PARA RESPOSTA HTTP ESTRUTURADA ---
    private static Task HandleExceptionAsync(HttpContext context, System.Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            KeyNotFoundException        => (HttpStatusCode.NotFound, ex.Message),
            InvalidOperationException   => (HttpStatusCode.BadRequest, ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
            _                           => (HttpStatusCode.InternalServerError, "Erro interno do servidor.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new ErrorResponse
        {
            Message = message,
            StatusCode = context.Response.StatusCode
        });

        return context.Response.WriteAsync(payload);
    }
}
