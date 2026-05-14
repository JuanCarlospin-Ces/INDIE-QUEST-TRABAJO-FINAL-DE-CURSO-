using NUnit.Framework;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Infrastructure;
using IndieQuest_Api.Infrastructure.Repository.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Tests.IntegrationTest;

public class PostgreSqlUserRepositoryCreateUserTests
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
    public async Task CreateUserAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var repository = new PostgreSqlUserRepository(_context);
        var initialCount = (await repository.GetAllUsersAsync()).Count;

        var newUser = new User
        {
            Username = "new_user",
            Password = "password123",
            Email = "newuser@example.com",
            AvailableForWork = true,
            dateOfRegistration = DateTime.UtcNow
        };

        // Act
        await repository.CreateUserAsync(newUser);
        var allUsers = await repository.GetAllUsersAsync();

        // Assert
        Assert.That(allUsers.Count, Is.EqualTo(initialCount + 1));
        Assert.That(allUsers.Any(u => u.Username == "new_user"), Is.True);
    }
}
