using System.Security.Claims;

namespace LiveEventsTicket.Backend.Infra.Security;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    // --- INJECAO DO ACESSOR DO CONTEXTO HTTP ---
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // --- OBTEM O ID DO USUARIO AUTENTICADO A PARTIR DOS CLAIMS DO JWT ---
    public int GetUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

        if (claim is null || !int.TryParse(claim, out var userId))
        {
            throw new InvalidOperationException("Usuário autenticado inválido.");
        }

        return userId;
    }
}
