using System;
using IndieQuest_Api.Domain.ValueObject;

namespace IndieQuest_Api.Domain.Model;

public class Post
{
    public string PostId { get; set; }
    public string PostUserId { get; set; }
    public string Title { get; set; }
    public string MediaContent { get; set; }
    
    public string? Description { get; set; }
    public DateTime CreationDate { get; set; }
    public Tag[]? Tags { get; set; }

}
