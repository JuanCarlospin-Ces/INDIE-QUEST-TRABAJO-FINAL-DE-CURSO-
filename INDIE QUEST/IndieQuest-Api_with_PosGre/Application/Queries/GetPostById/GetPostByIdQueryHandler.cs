using System;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Queries.GetPostById;

public class GetPostByIdQueryHandler
{
    private readonly IPostRepository _postRepository;

    public GetPostByIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Post?> Handle(int postId)
    {
        return await _postRepository.GetPostByIdAsync(postId);
    }
}
