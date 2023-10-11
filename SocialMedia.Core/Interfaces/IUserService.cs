using SocialMedia.Core.Entities.Users;

namespace SocialMedia.Core.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> Search(string firstName, string secondName);
}