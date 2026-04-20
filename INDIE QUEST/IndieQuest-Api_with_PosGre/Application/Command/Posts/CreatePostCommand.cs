using System;
using IndieQuest_Api.Domain.ValueObject;

namespace IndieQuest_Api.Application.Command.Posts;

public class CreatePostCommand
{
    public int UserId { get; set; }
    public required string Title { get; set; }
    public required string MediaContent { get; set; }
    public string? Description { get; set; }
    public int[]? TagIds { get; set; }
}
