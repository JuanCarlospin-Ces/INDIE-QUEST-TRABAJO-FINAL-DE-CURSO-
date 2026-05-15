using System;
using IndieQuest_Api.Application;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Queries.GetAllUsers;

public class GetAllUsersQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> Handle()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<PagedResult<User>> Handle(int pageNumber, int pageSize, bool? availableForWork = null)
    {
        var (items, totalCount) = await _userRepository.GetAllUsersPagedAsync(pageNumber, pageSize, availableForWork);
        return new PagedResult<User>
        {
            Data       = items,
            PageNumber = pageNumber,
            PageSize   = pageSize,
            TotalCount = totalCount
        };
    }
}

