using SocialMedia.ApiModels.DialogMessages;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Services;

public class DialogService: IDialogService
{
    private readonly IDialogMessageRepository _dialogMessageRepository;

    public DialogService(IDialogMessageRepository dialogMessageRepository)
    {
        _dialogMessageRepository = dialogMessageRepository;
    }

    public async Task<IEnumerable<DialogMessageDto>> GetDialogMessages(int userIdFrom, int userIdTo) =>
        (await _dialogMessageRepository.GetDialogMessages(userIdFrom, userIdTo)).Select(dm=>dm.ToDto());

    public async Task<DialogMessageDto> SendMessage(int userId, DialogMessageDto dialogMessageDto)
    {
        var dialogMessage = dialogMessageDto.ToDbEntry();
        dialogMessage.UserIdTo = userId;
        var dialogMessageId = await _dialogMessageRepository.AddAsync(dialogMessage);
        dialogMessage.Id = dialogMessageId;
        return dialogMessage.ToDto();
    }
}