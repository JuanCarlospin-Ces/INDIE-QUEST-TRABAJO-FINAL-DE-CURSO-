using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryUserRepositoryUpdateUserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task UpdateUserAsync_ShouldUpdateExistingUser()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var existingUser = repository.Users.First();

        var updatedUser = new User
        {
            UserId = existingUser.UserId,
            Username = "updated_username",
            Password = "updatedpass",
            Email = "updated@example.com",
            AvailableForWork = false,
            UserBio = "Updated bio"
        };

        // Act
        await repository.UpdateUserAsync(updatedUser);
        var result = await repository.GetUserByIdAsync(existingUser.UserId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("updated_username"));
        Assert.That(result.Email, Is.EqualTo("updated@example.com"));
        Assert.That(result.UserBio, Is.EqualTo("Updated bio"));
    }

    [Test]
    public async Task UpdateUserAsync_ShouldNotThrow_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var nonExistentUser = new User
        {
            UserId = "nonexistent-id",
            Username = "ghost",
            Password = "pass",
            Email = "ghost@example.com"
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await repository.UpdateUserAsync(nonExistentUser));
    }
}
