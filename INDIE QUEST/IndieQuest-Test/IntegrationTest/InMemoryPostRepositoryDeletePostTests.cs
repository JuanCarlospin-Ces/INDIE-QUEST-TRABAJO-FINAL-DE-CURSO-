using NUnit.Framework;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryPostRepositoryDeletePostTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task DeletePostAsync_ShouldRemovePost_WhenPostExists()
    {
        // Arrange
        var repository = new InMemoryPostRepository();
        var existingPost = repository.Posts.First();
        var initialCount = repository.Posts.Count;

        // Act
        await repository.DeletePostAsync(existingPost.PostId);
        var allPosts = await repository.GetAllPostsAsync();

        // Assert
        Assert.That(allPosts.Count, Is.EqualTo(initialCount - 1));
        Assert.That(allPosts.Any(p => p.PostId == existingPost.PostId), Is.False);
    }

    [Test]
    public async Task DeletePostAsync_ShouldNotThrow_WhenPostDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryPostRepository();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await repository.DeletePostAsync("nonexistent-id"));
    }
}
