using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryPostRepositoryUpdatePostTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task UpdatePostAsync_ShouldUpdateExistingPost()
    {
        // Arrange
        var repository = new InMemoryPostRepository();
        var existingPost = repository.Posts.First();

        var updatedPost = new Post
        {
            PostId = existingPost.PostId,
            PostUserId = existingPost.PostUserId,
            Title = "Updated Title",
            MediaContent = "https://example.com/updated.jpg",
            Description = "Updated description",
            CreationDate = existingPost.CreationDate
        };

        // Act
        await repository.UpdatePostAsync(updatedPost);
        var result = await repository.GetPostByIdAsync(existingPost.PostId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("Updated Title"));
        Assert.That(result.MediaContent, Is.EqualTo("https://example.com/updated.jpg"));
        Assert.That(result.Description, Is.EqualTo("Updated description"));
    }

    [Test]
    public async Task UpdatePostAsync_ShouldNotThrow_WhenPostDoesNotExist()
    {
        // Arrange
        var repository = new InMemoryPostRepository();
        var nonExistentPost = new Post
        {
            PostId = "nonexistent-id",
            PostUserId = "user-1",
            Title = "Title",
            MediaContent = "url"
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await repository.UpdatePostAsync(nonExistentPost));
    }
}
