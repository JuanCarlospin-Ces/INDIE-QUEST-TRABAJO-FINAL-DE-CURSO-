using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Tests.IntegrationTest;

public class PostgreSqlUserRepositoryUpdateUserTests
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
            new User { Username = "bob", Password = "pass2", Email = "bob@example.com", dateOfRegistration = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task UpdateUserAsync_ShouldUpdateExistingUser()
    {
        // Arrange
        var repository = new PostgreSqlUserRepository(_context);
        var existingUserId = _context.Users.First().UserId;
        var existingRegistration = _context.Users.First().dateOfRegistration;

        // Clear the change tracker so that the seeded tracked entities do not conflict
        // when PostgreSqlUserRepository calls _context.Users.Update() with a new instance
        _context.ChangeTracker.Clear();

        var updatedUser = new User
        {
            UserId = existingUserId,
            Username = "updated_username",
            Password = "updatedpass",
            Email = "updated@example.com",
            AvailableForWork = false,
            UserBio = "Updated bio",
            dateOfRegistration = existingRegistration
        };

        // Act
        await repository.UpdateUserAsync(updatedUser);
        var result = await repository.GetUserByIdAsync(existingUserId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("updated_username"));
        Assert.That(result.Email, Is.EqualTo("updated@example.com"));
        Assert.That(result.UserBio, Is.EqualTo("Updated bio"));
    }

    [Test]
    public void UpdateUserAsync_ShouldThrowConcurrencyException_WhenUserDoesNotExist()
    {
        // Arrange
        var repository = new PostgreSqlUserRepository(_context);
        var nonExistentUser = new User
        {
            UserId = 9999,
            Username = "ghost",
            Password = "pass",
            Email = "ghost@example.com",
            dateOfRegistration = DateTime.UtcNow
        };

        // PostgreSqlUserRepository uses _context.Users.Update() directly, which throws
        // DbUpdateConcurrencyException when the entity does not exist in the database
        Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>(
            async () => await repository.UpdateUserAsync(nonExistentUser));
    }
}
