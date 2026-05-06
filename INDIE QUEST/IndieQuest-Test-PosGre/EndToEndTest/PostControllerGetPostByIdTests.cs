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

namespace IndieQuest_Test_PosGre.EndToEndTest;

public class PostControllerGetPostByIdTests
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
    public async Task GetPostById_ShouldReturnOk_WhenPostExists()
    {
        // Arrange
        var post = new Post { PostId = 1, Title = "Title", MediaContent = "url", CreationDate = DateTime.UtcNow };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync(1)).ReturnsAsync(post);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetPostById(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<Post>());
        var returnedPost = (Post)okResult.Value!;
        Assert.That(returnedPost.PostId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetPostById_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync(99)).ReturnsAsync((Post?)null);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetPostById(99);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
