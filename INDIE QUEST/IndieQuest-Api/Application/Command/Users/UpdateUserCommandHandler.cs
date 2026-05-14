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

        // Modificar la entidad rastreada en lugar de crear una nueva instancia
        // (crear una instancia nueva con el mismo ID produce un conflicto de tracking en EF Core)
        existingUser.Username          = command.Username;
        existingUser.Password          = command.Password;
        existingUser.AvailableForWork  = command.AvailableForWork;
        existingUser.UserBio           = command.UserBio;
        existingUser.UserProfilePicture = command.UserProfilePicture;
        existingUser.Email             = command.Email;

        await _userRepository.UpdateUserAsync(existingUser);
        return existingUser;
    }
}
