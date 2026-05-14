using System;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Command.Users;

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
