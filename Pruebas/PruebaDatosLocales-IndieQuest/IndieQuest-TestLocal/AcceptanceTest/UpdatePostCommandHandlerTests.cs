using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.AcceptanceTest;

public class UpdatePostCommandHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldUpdatePost_WhenPostExists()
    {
        // Arrange
        var existingPost = new Post
        {
            PostId = "post-1",
            PostUserId = "user-1",
            Title = "Original Title",
            MediaContent = "https://example.com/old.jpg",
            CreationDate = DateTime.UtcNow
        };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync("post-1")).ReturnsAsync(existingPost);
        mockRepo.Setup(r => r.UpdatePostAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

        var handler = new UpdatePostCommandHandler(mockRepo.Object);

        var command = new UpdatePostCommand
        {
            PostId = "post-1",
            Title = "Updated Title",
            MediaContent = "https://example.com/new.jpg",
            Description = "Updated description"
        };

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("Updated Title"));
        Assert.That(result.MediaContent, Is.EqualTo("https://example.com/new.jpg"));
        mockRepo.Verify(r => r.UpdatePostAsync(It.IsAny<Post>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnNull_WhenPostDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync("nonexistent")).ReturnsAsync((Post?)null);

        var handler = new UpdatePostCommandHandler(mockRepo.Object);

        var command = new UpdatePostCommand
        {
            PostId = "nonexistent",
            Title = "Title",
            MediaContent = "https://example.com/image.jpg"
        };

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.That(result, Is.Null);
        mockRepo.Verify(r => r.UpdatePostAsync(It.IsAny<Post>()), Times.Never);
    }
}
