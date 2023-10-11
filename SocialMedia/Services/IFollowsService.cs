namespace SocialMedia.Services;

public interface IFollowsService
{
    Task AddFollower(int followerId, int followeeId);
    Task DeleteFollower(int followerId, int followeeId);
}