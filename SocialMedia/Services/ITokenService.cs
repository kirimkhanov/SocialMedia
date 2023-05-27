using System.Security.Claims;

namespace SocialMedia.Services;

public interface ITokenService
{
    string CreateToken(ClaimsIdentity identity);
}