using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Test_PosGre.IntegrationTest;

public class PostgreSqlPostRepositoryGetAllPostsTests
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
    public async Task GetAllPostsAsync_ShouldReturnAllSeededPosts()
    {
        // Arrange
        var repository = new PostgreSqlPostRepository(_context);

        // Act
        var result = await repository.GetAllPostsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
    }
}
