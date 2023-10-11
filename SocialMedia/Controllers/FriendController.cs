using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Services;

namespace SocialMedia.Controllers;

[Authorize]
[Route("[controller]")]
public class FriendController : ControllerBase
{
    private readonly IFollowsService _followsService;
    private readonly IAuthService _authService;

    public FriendController(IFollowsService followsService, IAuthService authService)
    {
        _followsService = followsService;
        _authService = authService;
    }

    [HttpPost("set/{id:int}")]
    public async Task<IActionResult> SetFriend(int id)
    {
        var currentUserId = _authService.GetCurrentUserId();
        if (currentUserId is null)
            return NotFound("Пользователь не найден");
        
        await _followsService.AddFollower(currentUserId.Value, id);
        return Ok("Пользователь успешно указал своего друга");
    }
    
    [HttpPost("delete/{id:int}")]
    public async Task<IActionResult> DeleteFriend(int id)
    {
        var currentUserId = _authService.GetCurrentUserId();
        if (currentUserId is null)
            return NotFound("Пользователь не найден");
        
        await _followsService.DeleteFollower(currentUserId.Value, id);
        return Ok("Пользователь успешно удалил из друзей пользователя");
    }
}