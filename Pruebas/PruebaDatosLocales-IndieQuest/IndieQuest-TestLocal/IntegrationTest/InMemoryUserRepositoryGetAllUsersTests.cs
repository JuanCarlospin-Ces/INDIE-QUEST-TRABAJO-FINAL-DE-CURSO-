using NUnit.Framework;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryUserRepositoryGetAllUsersTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetAllUsersAsync_ShouldReturnPreloadedUsers()
    {
        // Arrange
        var repository = new InMemoryUserRepository();

        // Act
        var result = await repository.GetAllUsersAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
    }
}
