namespace SocialMedia.Core.Entities;

public class DialogMessage: BaseEntity
{
    public int Id { get; set; }
    public int UserIdFrom { get; set; }
    public int UserIdTo { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}