using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.ApiModels;
using SocialMedia.Core.Interfaces;
using SocialMedia.Utils;

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
    public async Task<IActionResult> Register(UserDTO userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest("Required form fields are not filled or invalid");

        var user = userDto.FromDTO(Encryptor.MD5Hash(userDto.Password));
        return Ok(await _userRepository.AddAsync(user));
    }
}