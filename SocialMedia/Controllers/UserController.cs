using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.ApiModels;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Controllers;

[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [Authorize]
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user is null)
            return NotFound($"User was not found by id = {id}");

        return Ok(user.ToDTO());
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<int> Register(UserDTO userDto) =>
        await _userRepository.AddAsync(userDto.FromDTO());
}