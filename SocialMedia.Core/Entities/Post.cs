﻿namespace SocialMedia.Core.Entities;

public class Post: BaseEntity
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}