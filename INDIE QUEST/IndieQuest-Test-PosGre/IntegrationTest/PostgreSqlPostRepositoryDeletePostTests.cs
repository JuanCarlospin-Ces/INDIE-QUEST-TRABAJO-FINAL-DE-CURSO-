using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Test_PosGre.IntegrationTest;

public class PostgreSqlPostRepositoryDeletePostTests
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
            new Post { Title = "Post 2", MediaContent = "url2", CreationDate = DateTime.UtcNow },
            new Post { Title = "Post 3", MediaContent = "url3", CreationDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task DeletePostAsync_ShouldRemovePost_WhenPostExists()
    {
        // Arrange
        var repository = new PostgreSqlPostRepository(_context);
        var existingPost = _context.Posts.First();
        var initialCount = _context.Posts.Count();

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
        var repository = new PostgreSqlPostRepository(_context);

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await repository.DeletePostAsync(9999));
    }
}
