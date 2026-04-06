using System;
using IQ_Api.Domain.Model;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Application.Queries.GetPostsByUserId;

public class GetPostsByUserIdQueryHandler
{
    private readonly IPostRepository _postRepository;

    public GetPostsByUserIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<Post>> Handle(string userId)
    {
        return await _postRepository.GetPostsByUserIdAsync(userId);
    }
}
