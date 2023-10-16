namespace SocialMedia.ApiModels.DialogMessages;

public class DialogMessageDto
{
    [SwaggerExclude]
    public int Id { get; set; }
    [SwaggerExclude]
    public int UserIdFrom { get; set; }
    [SwaggerExclude]
    public int UserIdTo { get; set; }
    public string Text { get; set; }
    [SwaggerExclude]
    public DateTime CreatedAt { get; set; }
}