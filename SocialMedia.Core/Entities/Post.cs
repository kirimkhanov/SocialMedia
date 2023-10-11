namespace SocialMedia.Core.Entities;

public class Post
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}