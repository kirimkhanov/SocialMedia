namespace SocialMedia.Core.Entities;

public class Follow
{
    public int FolloweeId { get; set; }
    public int FollowerId { get; set; }
    public bool IsDeleted { get; set; }
}