using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Queries.GetPostsByUserId;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.UnitTest;

public class GetPostsByUserIdQueryHandlerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldReturnPosts_WhenUserHasPosts()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post { PostId = "post-1", PostUserId = "user-1", Title = "Post 1", MediaContent = "url1", CreationDate = DateTime.UtcNow },
            new Post { PostId = "post-2", PostUserId = "user-1", Title = "Post 2", MediaContent = "url2", CreationDate = DateTime.UtcNow }
        };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostsByUserIdAsync("user-1")).ReturnsAsync(posts);

        var handler = new GetPostsByUserIdQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle("user-1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result, Has.All.Matches<Post>(p => p.PostUserId == "user-1"));
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoPosts()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostsByUserIdAsync("user-99")).ReturnsAsync(new List<Post>());

        var handler = new GetPostsByUserIdQueryHandler(mockRepo.Object);

        // Act
        var result = await handler.Handle("user-99");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
