using SocialMedia.Core.Entities;

namespace SocialMedia.ApiModels.Posts;

public static class PostDtoConverter
{
    public static PostDto ToDto(this Post post) =>
        new()
        {
            Id = post.Id,
            Text = post.Text,
            UserId = post.UserId
        };

    public static Post ToDbEntry(this PostDto postDto) =>
        new()
        {
            Id = postDto.Id,
            Text = postDto.Text,
            CreatedAt = DateTime.Now,
            UserId = postDto.UserId
        };
}