using SocialMedia.Core.Entities;

namespace SocialMedia.Core.Interfaces;

public interface IDialogMessageRepository
{
    Task<IEnumerable<DialogMessage>> GetDialogMessages(int userIdFrom, int userIdTo);
    Task<int> AddAsync(DialogMessage dialogMessage);
}