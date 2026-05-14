using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.AcceptanceTest;

public class CreateUserCommandHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldCreateUser_WhenCommandIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var handler = new CreateUserCommandHandler(mockRepo.Object);

        var command = new CreateUserCommand
        {
            UserId = "user-1",
            Username = "testuser",
            Password = "password123",
            Email = "test@example.com",
            AvailableForWork = true,
            UserBio = "Developer"
        };

        // Act
        await handler.Handle(command);

        // Assert
        mockRepo.Verify(r => r.CreateUserAsync(It.Is<User>(u =>
            u.UserId == command.UserId &&
            u.Username == command.Username &&
            u.Email == command.Email &&
            u.AvailableForWork == command.AvailableForWork
        )), Times.Once);
    }
}
