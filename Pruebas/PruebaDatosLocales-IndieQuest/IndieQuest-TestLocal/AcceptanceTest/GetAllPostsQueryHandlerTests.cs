using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Queries.GetAllPosts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.AcceptanceTest;

public class GetAllPostsQueryHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldReturnAllPosts()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post { PostId = "post-1", PostUserId = "user-1", Title = "Post 1", MediaContent = "url1", CreationDate = DateTime.UtcNow },
            new Post { PostId = "post-2", PostUserId = "user-2", Title = "Post 2", MediaContent = "url2", CreationDate = DateTime.UtcNow }
        };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetAllPostsAsync()).ReturnsAsync(posts);

        var handler = new GetAllPostsQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPostsExist()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetAllPostsAsync()).ReturnsAsync(new List<Post>());

        var handler = new GetAllPostsQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
