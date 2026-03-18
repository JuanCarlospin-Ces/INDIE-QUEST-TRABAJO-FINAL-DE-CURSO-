using System;

namespace IQ_Api.Domain.Model;

public class Post
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    
}
