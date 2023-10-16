using SocialMedia.Core.Entities;

namespace SocialMedia.ApiModels.DialogMessages;

public static class DialogMessageDtoConverter
{
    public static DialogMessageDto ToDto(this DialogMessage dialogMessage) =>
        new()
        {
            Id = dialogMessage.Id,
            UserIdFrom = dialogMessage.UserIdFrom,
            UserIdTo = dialogMessage.UserIdTo,
            Text = dialogMessage.Text,
            CreatedAt = dialogMessage.CreatedAt
        };
    
    public static DialogMessage ToDbEntry(this DialogMessageDto dialogMessage) =>
        new()
        {
            UserIdFrom = dialogMessage.UserIdFrom,
            UserIdTo = dialogMessage.UserIdTo,
            Text = dialogMessage.Text,
            CreatedAt = DateTime.Now
        };
}