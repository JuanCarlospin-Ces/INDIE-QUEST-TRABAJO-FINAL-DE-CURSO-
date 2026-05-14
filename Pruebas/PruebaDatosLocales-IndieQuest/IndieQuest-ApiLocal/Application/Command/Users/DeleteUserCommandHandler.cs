using System;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Command.Users;

public class DeleteUserCommandHandler
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(string userId)
    {
        await _userRepository.DeleteUserAsync(userId);
    }
}
