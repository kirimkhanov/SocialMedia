namespace SocialMedia.Core.Entities.Users;

public class PostSearchParams
{
    public int UserId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; } = 10;
}