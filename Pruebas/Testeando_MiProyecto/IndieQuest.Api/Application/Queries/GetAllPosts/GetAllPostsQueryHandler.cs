using System;
using IQ_Api.Domain.Model;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Application.Queries.GetAllPosts;

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

