using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.ApiModels.Posts;
using SocialMedia.Core.Entities.Users;
using SocialMedia.Services;

namespace SocialMedia.Controllers;

[Authorize]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostsService _postsService;
    private readonly IPostsCacheManager _postsCacheManager;
    private readonly ILogger<PostController> _logger;

    public PostController(IPostsService postsService, IPostsCacheManager postsCacheManager,
        ILogger<PostController> logger)
    {
        _postsService = postsService;
        _postsCacheManager = postsCacheManager;
        _logger = logger;
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetPosts(PostSearchParams searchParams)
    {
        var posts = await _postsCacheManager.GetPosts(searchParams);
        return Ok(posts.Select(p => p.ToDto()));
    }

    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetPost(int id)
    {
        var post = await _postsService.GetPost(id);
        if (post is null)
            return NotFound("Пост не найден");

        return Ok(post.ToDto());
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePost(PostDto postDto)
    {
        await _postsService.CreatePost(postDto);
        return Ok("Успешно создан пост");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdatePost(PostDto postDto)
    {
        await _postsService.UpdatePost(postDto);
        return Ok("Успешно изменен пост");
    }

    [HttpPut("delete/{id:int}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        await _postsService.DeletePost(id);
        return Ok("Успешно удален пост");
    }
}