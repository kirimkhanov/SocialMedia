using SocialMedia.Core.Entities;

namespace SocialMedia.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(int userId);
    Task<IEnumerable<User>> GetUsers();
    Task<int> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}