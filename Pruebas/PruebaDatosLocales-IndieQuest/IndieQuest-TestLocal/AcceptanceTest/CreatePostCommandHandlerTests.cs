using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.AcceptanceTest;

public class CreatePostCommandHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldCreatePost_WhenCommandIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.CreatePostAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

        var handler = new CreatePostCommandHandler(mockRepo.Object);

        var command = new CreatePostCommand
        {
            PostId = "post-1",
            PostUserId = "user-1",
            Title = "Test Post",
            MediaContent = "https://example.com/image.jpg",
            Description = "A test post"
        };

        // Act
        await handler.Handle(command);

        // Assert
        mockRepo.Verify(r => r.CreatePostAsync(It.Is<Post>(p =>
            p.PostId == command.PostId &&
            p.PostUserId == command.PostUserId &&
            p.Title == command.Title &&
            p.MediaContent == command.MediaContent &&
            p.Description == command.Description
        )), Times.Once);
    }
}
