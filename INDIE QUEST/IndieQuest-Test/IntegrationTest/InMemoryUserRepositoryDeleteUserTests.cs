using NUnit.Framework;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryUserRepositoryDeleteUserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task DeleteUserAsync_ShouldRemoveUser_WhenUserExists()
    {
        // Arrange
        var repository = new InMemoryUserRepository();
        var existingUser = repository.Users.First();
        var initialCount = repository.Users.Count;

        // Act
        await repository.DeleteUserAsync(existingUser.UserId);
        var allUsers = await repository.GetAllUsersAsync();

        // Assert
        Assert.That(allUsers.Count, Is.EqualTo(initialCount - 1));
        Assert.That(allUsers.Any(u => u.UserId == existingUser.UserId), Is.False);
    }

    [Test]
    public async Task DeleteUserAsync_ShouldNotThrow_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryUserRepository();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await repository.DeleteUserAsync("nonexistent-id"));
    }
}
