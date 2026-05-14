using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Tests.IntegrationTest;

public class PostgreSqlUserRepositoryGetAllUsersTests
{
    private IndieQuestDbContext _context = null!;

    [SetUp]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<IndieQuestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new IndieQuestDbContext(options);

        // Seed initial users
        _context.Users.AddRange(
            new User { Username = "alice", Password = "pass1", Email = "alice@example.com", dateOfRegistration = DateTime.UtcNow },
            new User { Username = "bob", Password = "pass2", Email = "bob@example.com", dateOfRegistration = DateTime.UtcNow },
            new User { Username = "carol", Password = "pass3", Email = "carol@example.com", dateOfRegistration = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllUsersAsync_ShouldReturnAllSeededUsers()
    {
        // Arrange
        var repository = new PostgreSqlUserRepository(_context);

        // Act
        var result = await repository.GetAllUsersAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
    }
}
