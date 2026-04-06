using System;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Application.Command.Posts;

public class DeletePostCommandHandler
{
    private readonly IPostRepository _postRepository;

    public DeletePostCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task Handle(string postId)
    {
        await _postRepository.DeletePostAsync(postId);
    }
}
