using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Test_PosGre.IntegrationTest;

public class PostgreSqlPostRepositoryCreatePostTests
{
    private IndieQuestDbContext _context = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<IndieQuestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new IndieQuestDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task CreatePostAsync_ShouldAddPostToDatabase()
    {
        // Arrange
        var repository = new PostgreSqlPostRepository(_context);
        var initialCount = (await repository.GetAllPostsAsync()).Count;

        var newPost = new Post
        {
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
        Assert.That(allPosts.Any(p => p.Title == "New Test Post"), Is.True);
    }
}
