using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Test_PosGre.IntegrationTest;

public class PostgreSqlUserRepositoryDeleteUserTests
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
    public async Task DeleteUserAsync_ShouldRemoveUser_WhenUserExists()
    {
        // Arrange
        var repository = new PostgreSqlUserRepository(_context);
        var existingUser = _context.Users.First();
        var initialCount = _context.Users.Count();

        // Act
        await repository.DeleteUserAsync(existingUser.UserId);
        var allUsers = await repository.GetAllUsersAsync();

        // Assert
        Assert.That(allUsers.Count, Is.EqualTo(initialCount - 1));
        Assert.That(allUsers.Any(u => u.UserId == existingUser.UserId), Is.False);
    }

    [Test]
    public async Task DeleteUserAsync_ShouldNotThrow_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new PostgreSqlUserRepository(_context);

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await repository.DeleteUserAsync(9999));
    }
}
