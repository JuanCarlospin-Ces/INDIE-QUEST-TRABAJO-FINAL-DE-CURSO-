using System;
using IQ_Api.Domain.Model;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Application.Queries.GetAllUsers;

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
}

