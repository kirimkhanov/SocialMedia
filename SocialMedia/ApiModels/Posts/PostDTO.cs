using System.Text.Json.Serialization;

namespace SocialMedia.ApiModels.Posts;

public class PostDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    [JsonPropertyName("author_user_id")] 
    public int UserId { get; set; }
}