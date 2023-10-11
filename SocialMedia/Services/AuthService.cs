using System.Security.Claims;

namespace SocialMedia.Services;

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetCurrentUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
            c.Type == ClaimsIdentity.DefaultNameClaimType);
        if (int.TryParse(claim?.Value, out var userId))
            return userId;

        return null;
    }
}