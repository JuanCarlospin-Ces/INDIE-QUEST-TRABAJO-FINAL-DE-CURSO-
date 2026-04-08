using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.AcceptanceTest;

public class UpdateUserCommandHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var existingUser = new User
        {
            UserId = "user-1",
            Username = "old_username",
            Password = "oldpass",
            Email = "old@example.com",
            dateOfRegistration = DateTime.UtcNow
        };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("user-1")).ReturnsAsync(existingUser);
        mockRepo.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var handler = new UpdateUserCommandHandler(mockRepo.Object);

        var command = new UpdateUserCommand
        {
            UserId = "user-1",
            Username = "new_username",
            Password = "newpass",
            Email = "new@example.com",
            AvailableForWork = false
        };

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("new_username"));
        Assert.That(result.Email, Is.EqualTo("new@example.com"));
        mockRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var handler = new UpdateUserCommandHandler(mockRepo.Object);

        var command = new UpdateUserCommand
        {
            UserId = "nonexistent",
            Username = "username",
            Password = "pass",
            Email = "email@example.com"
        };

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result, Is.Null);
        mockRepo.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
    }
}
