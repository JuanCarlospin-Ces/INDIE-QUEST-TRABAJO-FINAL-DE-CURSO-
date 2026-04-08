using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.AcceptanceTest;

public class DeletePostCommandHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldDeletePost_WhenPostIdIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.DeletePostAsync("post-1")).Returns(Task.CompletedTask);

        var handler = new DeletePostCommandHandler(mockRepo.Object);

        // Act
        await handler.Handle("post-1");

        // Assert
        mockRepo.Verify(r => r.DeletePostAsync("post-1"), Times.Once);
    }
}
