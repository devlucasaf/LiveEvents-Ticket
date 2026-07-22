using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Exception;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    // --- OPCOES DE SERIALIZACAO JSON EM CAMELCASE ---
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // --- RECEBE O PROXIMO MIDDLEWARE E O LOGGER VIA INJECAO DE DEPENDENCIA ---
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
    private async Task HandleExceptionAsync(HttpContext context, System.Exception ex)
    {
        var traceId = context.TraceIdentifier;

        var (statusCode, message) = MapearExcecao(ex);

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(ex, "Erro NÃO tratado. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(ex, "Excecao de negocio ({StatusCode}). TraceId={TraceId} Path={Path}", (int)statusCode, traceId, context.Request.Path);
        }

        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // --- SERIALIZA O CORPO DE ERRO PADRONIZADO COM O TRACEID ---
        var payload = JsonSerializer.Serialize(new ErrorResponse
        {
            Message = message,
            StatusCode = context.Response.StatusCode,
            TraceId = traceId
        }, _jsonOptions);

        await context.Response.WriteAsync(payload);
    }

    // --- MAPEIA CADA TIPO DE EXCECAO PARA UM PAR SEGURO ---
    private static (HttpStatusCode statusCode, string message) MapearExcecao(System.Exception ex)
    {
        return ex switch
        {
            // --- RECURSO NÃO ENCONTRADO ---
            KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),

            // --- REGRA DE NEGOCIO/ESTADO INVALIDO ---
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),

            // --- ARGUMENTO INVALIDO INFORMADO NA REQUISICAO ---
            ArgumentException => (HttpStatusCode.BadRequest, ex.Message),

            // --- ACESSO NÃO AUTORIZADO A RECURSO PROTEGIDO ---
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Acesso não autorizado."),

            // --- CONFLITO DE INTEGRIDADE DO BANCO  ---
            DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Conflito ao atualizar o recurso. Tente novamente."),
            DbUpdateException            => (HttpStatusCode.Conflict, "Não foi possível persistir a operação."),

            // --- FUNCIONALIDADE AINDA NÃO IMPLEMENTADA ---
            NotImplementedException => (HttpStatusCode.NotImplemented, "Funcionalidade não implementada."),

            // --- REQUISICAO CANCELADA PELO CLIENTE OU POR TIMEOUT ---
            TaskCanceledException      => (HttpStatusCode.RequestTimeout, "A requisição expirou ou foi cancelada."),
            OperationCanceledException => (HttpStatusCode.RequestTimeout, "A requisição expirou ou foi cancelada."),

            // --- NÃO VAZAR DETALHES INTERNOS AO CLIENTE ---
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor.")
        };
    }
}
