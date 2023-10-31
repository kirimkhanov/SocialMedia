using SocialMedia.Core.Entities;

namespace SocialMedia.Services;

public interface IPostCreationHandler
{
    Task Handle(Post post);
}