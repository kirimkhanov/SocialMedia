using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core.Interfaces;
using SocialMedia.Services;
using SocialMedia.Utils;

namespace SocialMedia.Controllers;

[AllowAnonymous]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginController(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(int id, string password)
    {
        var identity = await GetIdentity(id, password);
        if (identity == null)
            return BadRequest(new { errorText = "Invalid username or password." });

        return new JsonResult(new
        {
            access_token = _tokenService.CreateToken(identity),
            username = identity.Name
        });
    }

    private async Task<ClaimsIdentity?> GetIdentity(int userId, string password)
    {
        var user = await _userRepository.GetUserById(userId);
        if (user == null || user.Password != Encryptor.MD5Hash(password)) return null;

        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString())
        };
        var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}