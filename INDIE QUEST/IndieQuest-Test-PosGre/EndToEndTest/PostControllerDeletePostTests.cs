using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using IndieQuest_Api.Controllers;
using IndieQuest_Api.Application.Queries.GetAllPosts;
using IndieQuest_Api.Application.Queries.GetPostById;
using IndieQuest_Api.Application.Queries.GetPostsByUserId;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Test_PosGre.EndToEndTest;

public class PostControllerDeletePostTests
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
    public async Task DeletePost_ShouldReturnOkWithMessage()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.DeletePostAsync(1)).Returns(Task.CompletedTask);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.DeletePost(1);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
        mockRepo.Verify(r => r.DeletePostAsync(1), Times.Once);
    }
}
