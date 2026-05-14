using System;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Queries.GetAllPosts;

public class GetAllPostsQueryHandler
{
    private readonly IPostRepository _postRepository;

    public GetAllPostsQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<Post>> Handle()
    {
        return await _postRepository.GetAllPostsAsync();
    }
}

