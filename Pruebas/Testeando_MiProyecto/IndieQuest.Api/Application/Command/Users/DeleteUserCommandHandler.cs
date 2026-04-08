using System;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Application.Command.Users;

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
