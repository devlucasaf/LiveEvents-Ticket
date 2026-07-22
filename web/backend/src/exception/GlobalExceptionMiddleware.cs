using System.Net;
using System.Text.Json;

namespace LiveEventsTicket.Backend.Exception;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    // --- RECEBE O PROXIMO MIDDLEWARE VIA INJECAO DE DEPENDENCIA ---
    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // --- EXECUTA O PIPELINE E INTERCEPTA QUALQUER EXCECAO LANCADA ---
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

    // --- CONVERTE A EXCECAO EM STATUS HTTP E CORPO JSON DE ERRO ---
    private static Task HandleExceptionAsync(HttpContext context, System.Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // --- SERIALIZA O CORPO DE ERRO PADRONIZADO ---
        var payload = JsonSerializer.Serialize(new ErrorResponse
        {
            Message = message,
            StatusCode = context.Response.StatusCode
        });

        return context.Response.WriteAsync(payload);
    }
}
