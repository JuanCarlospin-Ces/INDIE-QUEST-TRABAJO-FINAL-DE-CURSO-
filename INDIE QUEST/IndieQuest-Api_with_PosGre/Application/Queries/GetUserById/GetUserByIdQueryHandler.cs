using System;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(int userId)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }
}
