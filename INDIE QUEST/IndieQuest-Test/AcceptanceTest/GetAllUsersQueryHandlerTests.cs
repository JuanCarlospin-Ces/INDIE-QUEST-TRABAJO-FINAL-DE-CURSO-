using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Queries.GetAllUsers;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.AcceptanceTest;

public class GetAllUsersQueryHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = "user-1", Username = "alice", Password = "pass1", Email = "alice@example.com" },
            new User { UserId = "user-2", Username = "bob", Password = "pass2", Email = "bob@example.com" }
        };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

        var handler = new GetAllUsersQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());

        var handler = new GetAllUsersQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
