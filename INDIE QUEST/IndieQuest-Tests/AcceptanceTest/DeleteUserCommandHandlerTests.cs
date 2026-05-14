using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Tests.AcceptanceTest;

public class DeleteUserCommandHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldDeleteUser_WhenUserIdIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.DeleteUserAsync(1)).Returns(Task.CompletedTask);

        var handler = new DeleteUserCommandHandler(mockRepo.Object);

        // Act
        await handler.Handle(1);

        // Assert
        mockRepo.Verify(r => r.DeleteUserAsync(1), Times.Once);
    }
}
