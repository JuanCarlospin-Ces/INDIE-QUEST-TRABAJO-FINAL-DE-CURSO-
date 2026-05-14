using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Tests.IntegrationTest;

public class PostgreSqlPostRepositoryUpdatePostTests
{
    private IndieQuestDbContext _context = null!;

    [SetUp]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<IndieQuestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new IndieQuestDbContext(options);

        // Seed initial posts
        _context.Posts.AddRange(
            new Post { Title = "Post 1", MediaContent = "url1", CreationDate = DateTime.UtcNow },
            new Post { Title = "Post 2", MediaContent = "url2", CreationDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task UpdatePostAsync_ShouldUpdateExistingPost()
    {
        // Arrange
        var repository = new PostgreSqlPostRepository(_context);
        var existingPost = _context.Posts.First();

        var updatedPost = new Post
        {
            PostId = existingPost.PostId,
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
        var repository = new PostgreSqlPostRepository(_context);
        var nonExistentPost = new Post
        {
            PostId = 9999,
            Title = "Title",
            MediaContent = "url"
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await repository.UpdatePostAsync(nonExistentPost));
    }
}
