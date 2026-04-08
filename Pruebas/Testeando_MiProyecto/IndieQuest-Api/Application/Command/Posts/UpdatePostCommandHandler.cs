using System;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Command.Posts;

public class UpdatePostCommandHandler
{
    private readonly IPostRepository _postRepository;

    public UpdatePostCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Post?> Handle(UpdatePostCommand command)
    {
        var existingPost = await _postRepository.GetPostByIdAsync(command.PostId);
        if (existingPost == null)
        {
            return null;
        }

        var post = new Post
        {
            PostId = command.PostId,
            PostUserId = existingPost.PostUserId,
            Title = command.Title,
            MediaContent = command.MediaContent,
            Description = command.Description,
            Tags = command.Tags,
            CreationDate = existingPost.CreationDate
        };

        await _postRepository.UpdatePostAsync(post);
        return post;
    }
}
