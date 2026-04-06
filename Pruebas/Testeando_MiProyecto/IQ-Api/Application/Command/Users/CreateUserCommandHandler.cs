using System;
using IQ_Api.Application.Command.Users;
using IQ_Api.Domain.Model;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Application.Command.Users;

public class CreateUserCommandHandler
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(CreateUserCommand command)
    {
        var user = new User
        {
            UserId = command.UserId,
            Username = command.Username,
            Password = command.Password,
            AvailableForWork = command.AvailableForWork,
            UserBio = command.UserBio,
            UserProfilePicture = command.UserProfilePicture,
            Email = command.Email,
            dateOfRegistration = DateTime.UtcNow
        };

        await _userRepository.CreateUserAsync(user);
    }
}
