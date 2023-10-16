namespace SocialMedia.Core.Entities;

public class Follow: BaseEntity
{
    public int FolloweeId { get; set; }
    public int FollowerId { get; set; }
}