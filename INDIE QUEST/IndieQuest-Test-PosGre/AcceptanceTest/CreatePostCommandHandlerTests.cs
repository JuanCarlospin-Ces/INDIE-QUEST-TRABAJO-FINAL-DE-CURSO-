using NUnit.Framework;
using Moq;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Test_PosGre.AcceptanceTest;

public class CreatePostCommandHandlerTests
{
    private IndieQuestDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<IndieQuestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new IndieQuestDbContext(options);
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Handle_ShouldCreatePost_WhenCommandIsValid()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mockRepo = new Mock<IPostRepository>();
        // Callback simulates the real repo: adds the post to context and saves it so the PostId is assigned
        // and EF Core can resolve the UserPost foreign key
        mockRepo.Setup(r => r.CreatePostAsync(It.IsAny<Post>()))
            .Callback<Post>(p =>
            {
                context.Posts.Add(p);
                context.SaveChanges();
            })
            .Returns(Task.CompletedTask);

        var handler = new CreatePostCommandHandler(mockRepo.Object, context);

        var command = new CreatePostCommand
        {
            UserId = 1,
            Title = "Test Post",
            MediaContent = "https://example.com/image.jpg",
            Description = "A test post"
        };

        // Act
        await handler.Handle(command);

        // Assert
        mockRepo.Verify(r => r.CreatePostAsync(It.Is<Post>(p =>
            p.Title == command.Title &&
            p.MediaContent == command.MediaContent &&
            p.Description == command.Description
        )), Times.Once);
    }
}
