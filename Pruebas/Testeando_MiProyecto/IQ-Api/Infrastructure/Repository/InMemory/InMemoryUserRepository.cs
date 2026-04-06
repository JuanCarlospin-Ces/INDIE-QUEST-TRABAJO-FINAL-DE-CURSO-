using System;
using IQ_Api.Domain.Model;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Infrastructure.Repository.InMemory;

public class InMemoryUserRepository : IUserRepository
{
    public List<User> Users { get; set; } = new()
    {
        new User
        {
            UserId = "userId1",
            Username = "john_doe",
            Password = "password123",
            AvailableForWork = true,
            UserBio = "Software developer with a passion for open-source projects.",
            UserProfilePicture = "https://example.com/profile/john_doe.jpg",
            Email = "john.doe@example.com"
        },
        new User
        {
            UserId = "userId2",
            Username = "jane_smith",
            Password = "securepassword",
            AvailableForWork = false,
            UserBio = "Graphic designer specializing in digital art and branding.",
            UserProfilePicture = "https://example.com/profile/jane_smith.jpg",
            Email = "jane.smith@example.com"
        },
        new User
        {
            UserId = "userId3",
            Username = "alice_wonder",
            Password = "alicepassword",
            AvailableForWork = true,
            UserBio = "Content creator and social media manager with a love for storytelling.",
            UserProfilePicture = "https://example.com/profile/alice_wonder.jpg",
            Email = "alice.wonder@example.com"
        }

    };

    public Task<List<User>> GetAllUsersAsync()
    {
        return Task.FromResult(Users);
    }

    public Task<User?> GetUserByIdAsync(string userId)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        return Task.FromResult(user);
    }

    public Task CreateUserAsync(User user)
    {
        Users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateUserAsync(User user)
    {
        var existingUser = Users.FirstOrDefault(u => u.UserId == user.UserId);
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.Password = user.Password;
            existingUser.AvailableForWork = user.AvailableForWork;
            existingUser.UserBio = user.UserBio;
            existingUser.UserProfilePicture = user.UserProfilePicture;
            existingUser.Email = user.Email;
        }
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(string userId)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        if (user != null)
        {
            Users.Remove(user);
        }
        return Task.CompletedTask;
    }
}

