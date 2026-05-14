using System;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Command.Posts;

public class CreatePostCommandHandler
{
    private readonly IPostRepository _postRepository;

    public CreatePostCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task Handle(CreatePostCommand command)
    {
        var post = new Post
        {
            PostId = command.PostId,
            PostUserId = command.PostUserId,
            Title = command.Title,
            MediaContent = command.MediaContent,
            Description = command.Description,
            Tags = command.Tags,
            CreationDate = DateTime.UtcNow
        };

        await _postRepository.CreatePostAsync(post);
    }
}
