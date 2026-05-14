using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Queries.GetPostById;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.UnitTest;

public class GetPostByIdQueryHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldReturnPost_WhenPostExists()
    {
        // Arrange
        var post = new Post
        {
            PostId = "post-1",
            PostUserId = "user-1",
            Title = "Test Post",
            MediaContent = "https://example.com/image.jpg",
            CreationDate = DateTime.UtcNow
        };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync("post-1")).ReturnsAsync(post);

        var handler = new GetPostByIdQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle("post-1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.PostId, Is.EqualTo("post-1"));
        Assert.That(result.Title, Is.EqualTo("Test Post"));
    }

    [Test]
    public async Task Handle_ShouldReturnNull_WhenPostDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync("nonexistent")).ReturnsAsync((Post?)null);

        var handler = new GetPostByIdQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle("nonexistent");

        // Assert
        Assert.That(result, Is.Null);
    }
}
