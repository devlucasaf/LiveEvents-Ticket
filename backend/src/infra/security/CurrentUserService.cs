using System.Security.Claims;

namespace LiveEventsTicket.Backend.Infra.Security;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

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
