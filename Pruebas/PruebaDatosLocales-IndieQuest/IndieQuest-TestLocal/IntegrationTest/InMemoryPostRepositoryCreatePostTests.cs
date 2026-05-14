using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryPostRepositoryCreatePostTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreatePostAsync_ShouldAddPostToList()
    {
        // Arrange
        var repository = new InMemoryPostRepository();
        var initialCount = repository.Posts.Count;

        var newPost = new Post
        {
            PostId = "new-post-id",
            PostUserId = "user-99",
            Title = "New Test Post",
            MediaContent = "https://example.com/new.jpg",
            Description = "A newly created post",
            CreationDate = DateTime.UtcNow
        };

        // Act
        await repository.CreatePostAsync(newPost);
        var allPosts = await repository.GetAllPostsAsync();

        // Assert
        Assert.That(allPosts.Count, Is.EqualTo(initialCount + 1));
        Assert.That(allPosts.Any(p => p.PostId == "new-post-id"), Is.True);
    }
}
