using System;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Queries.GetPostsByUserId;

public class GetPostsByUserIdQueryHandler
{
    private readonly IPostRepository _postRepository;

    public GetPostsByUserIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<Post>> Handle(int userId)
    {
        return await _postRepository.GetPostsByUserIdAsync(userId);
    }
}
