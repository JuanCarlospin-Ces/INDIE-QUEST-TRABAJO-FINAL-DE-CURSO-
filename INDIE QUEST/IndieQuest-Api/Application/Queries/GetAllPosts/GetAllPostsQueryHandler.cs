using System;
using IndieQuest_Api.Application;
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

    public async Task<PagedResult<Post>> Handle(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _postRepository.GetAllPostsPagedAsync(pageNumber, pageSize);
        return new PagedResult<Post>
        {
            Data       = items,
            PageNumber = pageNumber,
            PageSize   = pageSize,
            TotalCount = totalCount
        };
    }
}

