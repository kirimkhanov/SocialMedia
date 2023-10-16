using SocialMedia.ApiModels.DialogMessages;

namespace SocialMedia.Services;

public interface IDialogService
{
    Task<IEnumerable<DialogMessageDto>> GetDialogMessages(int userIdFrom, int userIdTo);
    Task<DialogMessageDto> SendMessage(int userId, DialogMessageDto dialogMessageDto);
}