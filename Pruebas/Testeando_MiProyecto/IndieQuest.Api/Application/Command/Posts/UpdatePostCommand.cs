using System;
using IQ_Api.Domain.ValueObject;

namespace IQ_Api.Application.Command.Posts;

public class UpdatePostCommand
{
    public string PostId { get; set; }
    public string Title { get; set; }
    public string MediaContent { get; set; }
    public string? Description { get; set; }
    public Tag[]? Tags { get; set; }
}
