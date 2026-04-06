using System;
using IQ_Api.Domain.Model;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Application.Queries.GetPostById;

public class GetPostByIdQueryHandler
{
    private readonly IPostRepository _postRepository;

    public GetPostByIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Post?> Handle(string postId)
    {
        return await _postRepository.GetPostByIdAsync(postId);
    }
}
