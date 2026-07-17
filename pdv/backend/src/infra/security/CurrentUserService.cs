using System.Security.Claims;

namespace PontoVenda.Backend.Infra.Security;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    // --- INJEÇÃO DO ACESSOR DE HTTP CONTEXT ---
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // --- OBTER O ID DO OPERADOR LOGADO A PARTIR DO TOKEN ---
    public int GetUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

        if (claim is null || !int.TryParse(claim, out var userId))
        {
            throw new InvalidOperationException("Operador autenticado inválido.");
        }

        return userId;
    }

    // --- OBTER O NOME DO OPERADOR LOGADO A PARTIR DO TOKEN ---
    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
               ?? "Operador";
    }
}
