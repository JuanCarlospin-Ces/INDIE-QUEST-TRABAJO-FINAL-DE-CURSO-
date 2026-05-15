using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using IndieQuest_Api.Controllers;
using IndieQuest_Api.Application.Queries.GetAllPosts;
using IndieQuest_Api.Application.Queries.GetPostById;
using IndieQuest_Api.Application.Queries.GetPostsByUserId;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Tests.EndToEndTest;

public class PostControllerCreatePostTests
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

    private PostController BuildController(Mock<IPostRepository> postRepoMock)
    {
        var getAllHandler = new GetAllPostsQueryHandler(postRepoMock.Object);
        var getByIdHandler = new GetPostByIdQueryHandler(postRepoMock.Object);
        var getByUserIdHandler = new GetPostsByUserIdQueryHandler(postRepoMock.Object);
        var createHandler = new CreatePostCommandHandler(postRepoMock.Object, _context);
        var updateHandler = new UpdatePostCommandHandler(postRepoMock.Object, _context);
        var deleteHandler = new DeletePostCommandHandler(postRepoMock.Object);

        return new PostController(getAllHandler, getByIdHandler, getByUserIdHandler, createHandler, updateHandler, deleteHandler);
    }

    [Test]
    public async Task CreatePost_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        // Callback simulates the real repo: adds the post to context and saves it so the PostId is assigned
        // and EF Core can resolve the UserPost foreign key
        mockRepo.Setup(r => r.CreatePostAsync(It.IsAny<Post>()))
            .Callback<Post>(p =>
            {
                _context.Posts.Add(p);
                _context.SaveChanges();
            })
            .Returns(Task.CompletedTask);

        var controller = BuildController(mockRepo);

        var command = new CreatePostCommand
        {
            UserId = 1,
            Title = "New Post",
            MediaContent = "https://example.com/image.jpg"
        };

        // Act
        var result = await controller.CreatePost(command);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        mockRepo.Verify(r => r.CreatePostAsync(It.IsAny<Post>()), Times.Once);
    }
}
