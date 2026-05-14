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

public class PostControllerGetAllPostsTests
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

    private PostController BuildController(Mock<IPostRepository>? postRepoMock = null)
    {
        postRepoMock ??= new Mock<IPostRepository>();

        var getAllHandler = new GetAllPostsQueryHandler(postRepoMock.Object);
        var getByIdHandler = new GetPostByIdQueryHandler(postRepoMock.Object);
        var getByUserIdHandler = new GetPostsByUserIdQueryHandler(postRepoMock.Object);
        var createHandler = new CreatePostCommandHandler(postRepoMock.Object, _context);
        var updateHandler = new UpdatePostCommandHandler(postRepoMock.Object, _context);
        var deleteHandler = new DeletePostCommandHandler(postRepoMock.Object);

        return new PostController(getAllHandler, getByIdHandler, getByUserIdHandler, createHandler, updateHandler, deleteHandler);
    }

    [Test]
    public async Task GetAllPosts_ShouldReturnOkWithPosts()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post { PostId = 1, Title = "Post 1", MediaContent = "url1", CreationDate = DateTime.UtcNow }
        };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetAllPostsAsync()).ReturnsAsync(posts);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetAllPosts();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<List<Post>>());
        var returnedPosts = (List<Post>)okResult.Value!;
        Assert.That(returnedPosts, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetAllPosts_ShouldReturnOkWithEmptyList_WhenNoPosts()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetAllPostsAsync()).ReturnsAsync(new List<Post>());

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetAllPosts();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<List<Post>>());
        var returnedPosts = (List<Post>)okResult.Value!;
        Assert.That(returnedPosts, Is.Empty);
    }
}
