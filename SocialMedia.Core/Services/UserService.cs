using SocialMedia.Core.Entities.Users;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Core.Services;

public class UserService:IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<IEnumerable<User>> Search(string firstName, string secondName)=>
        _userRepository.Search(firstName, secondName);
}