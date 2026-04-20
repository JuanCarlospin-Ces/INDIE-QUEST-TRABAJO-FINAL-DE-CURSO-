using System;
using IndieQuest_Api.Domain.ValueObject;

namespace IndieQuest_Api.Application.Command.Posts;

public class UpdatePostCommand
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string MediaContent { get; set; }
    public string? Description { get; set; }
    public int[]? TagIds { get; set; }
}
