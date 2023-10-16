using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.ApiModels.DialogMessages;
using SocialMedia.Services;

namespace SocialMedia.Controllers;

[Authorize]
[Route("[controller]")]
public class DialogController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;

    public DialogController(IDialogService dialogService, IAuthService authService)
    {
        _dialogService = dialogService;
        _authService = authService;
    }

    [HttpGet("{userId}/list")]
    public async Task<IActionResult> GetUserMessages(int userId)
    {
        var currentUserId = _authService.GetCurrentUserId();
        if (currentUserId is null)
            return Unauthorized("Пользователь не найден");
        
        return Ok(await _dialogService.GetDialogMessages(currentUserId.Value, userId));
    }
    
    [HttpPost("{userId}/send")]
    public async Task<IActionResult> SendMessage(int userId, DialogMessageDto dialogMessageDto)
    {
        var currentUserId = _authService.GetCurrentUserId();
        if (currentUserId is null)
            return Unauthorized("Пользователь не найден");

        dialogMessageDto.UserIdFrom = currentUserId.Value;
        var dialogMessageResponseDto = await _dialogService.SendMessage(userId, dialogMessageDto);
        return Ok(dialogMessageResponseDto);
    }
}