using NUnit.Framework;
using IndieQuest_Api.Infrastructure.Repository.InMemory;

namespace IndieQuest_Test.IntegrationTest;

public class InMemoryPostRepositoryGetAllPostsTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetAllPostsAsync_ShouldReturnPreloadedPosts()
    {
        // Arrange
        var repository = new InMemoryPostRepository();

        // Act
        var result = await repository.GetAllPostsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
    }
}
