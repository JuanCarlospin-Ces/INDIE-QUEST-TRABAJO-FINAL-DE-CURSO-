using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryUserRepositoryCreateUserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUserAsync_ShouldAddUserToList()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var initialCount = repository.Users.Count;

        var newUser = new User
        {
            UserId = "new-user-id",
            Username = "new_user",
            Password = "password123",
            Email = "newuser@example.com",
            AvailableForWork = true,
            dateOfRegistration = DateTime.UtcNow
        };

        // Act
        await repository.CreateUserAsync(newUser);
        var allUsers = await repository.GetAllUsersAsync();

        // Assert
        Assert.That(allUsers.Count, Is.EqualTo(initialCount + 1));
        Assert.That(allUsers.Any(u => u.UserId == "new-user-id"), Is.True);
    }
}
