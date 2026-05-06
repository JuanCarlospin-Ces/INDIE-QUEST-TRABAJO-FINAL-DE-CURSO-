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

public class PostControllerUpdatePostTests
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
    public async Task UpdatePost_ShouldReturnOk_WhenPostExists()
    {
        // Arrange
        var existingPost = new Post { PostId = 1, Title = "Old", MediaContent = "url", CreationDate = DateTime.UtcNow };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync(1)).ReturnsAsync(existingPost);
        mockRepo.Setup(r => r.UpdatePostAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

        var controller = BuildController(mockRepo);

        var command = new UpdatePostCommand
        {
            PostId = 1,
            Title = "Updated Title",
            MediaContent = "https://example.com/updated.jpg"
        };

        // Act
        var result = await controller.UpdatePost(1, command);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
    }

    [Test]
    public async Task UpdatePost_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync(99)).ReturnsAsync((Post?)null);

        var controller = BuildController(mockRepo);

        var command = new UpdatePostCommand
        {
            PostId = 99,
            Title = "Title",
            MediaContent = "url"
        };

        // Act
        var result = await controller.UpdatePost(99, command);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
