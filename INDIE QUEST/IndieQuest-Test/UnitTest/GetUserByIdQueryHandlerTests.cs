using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Queries.GetUserById;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.UnitTest;

public class GetUserByIdQueryHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            UserId = "user-1",
            Username = "alice",
            Password = "password123",
            Email = "alice@example.com",
            AvailableForWork = true
        };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("user-1")).ReturnsAsync(user);

        var handler = new GetUserByIdQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle("user-1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserId, Is.EqualTo("user-1"));
        Assert.That(result.Username, Is.EqualTo("alice"));
    }

    [Test]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var handler = new GetUserByIdQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle("nonexistent");

        // Assert
        Assert.That(result, Is.Null);
    }
}
