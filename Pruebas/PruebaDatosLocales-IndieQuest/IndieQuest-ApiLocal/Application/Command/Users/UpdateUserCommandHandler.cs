using System;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Api.Application.Command.Users;

public class UpdateUserCommandHandler
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(UpdateUserCommand command)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(command.UserId);
        if (existingUser == null)
        {
            return null;
        }

        var user = new User
        {
            UserId = command.UserId,
            Username = command.Username,
            Password = command.Password,
            AvailableForWork = command.AvailableForWork,
            UserBio = command.UserBio,
            UserProfilePicture = command.UserProfilePicture,
            Email = command.Email,
            dateOfRegistration = existingUser.dateOfRegistration
        };

        await _userRepository.UpdateUserAsync(user);
        return user;
    }
}
