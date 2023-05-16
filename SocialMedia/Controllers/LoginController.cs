using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Controllers;

[AllowAnonymous]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public LoginController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(int id, string password)
    {
        var identity = await GetIdentity(id, password);
        if (identity == null)
            return BadRequest(new { errorText = "Invalid username or password." });

        var now = DateTime.UtcNow;
        
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var response = new
        {
            access_token = encodedJwt,
            username = identity.Name
        };

        return new JsonResult(response);
    }

    private async Task<ClaimsIdentity?> GetIdentity(int userId, string password)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user == null || user.Password != password) return null;

        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, user.Id.ToString())
        };
        var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}