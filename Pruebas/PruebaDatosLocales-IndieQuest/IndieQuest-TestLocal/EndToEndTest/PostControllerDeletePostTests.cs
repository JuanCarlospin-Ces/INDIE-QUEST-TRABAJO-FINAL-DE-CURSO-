using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using IndieQuest_Api.Controllers;
using IndieQuest_Api.Application.Queries.GetAllPosts;
using IndieQuest_Api.Application.Queries.GetPostById;
using IndieQuest_Api.Application.Queries.GetPostsByUserId;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.EndToEndTest;

public class PostControllerDeletePostTests
{
    [SetUp]
    public void Setup()
    {
    }

    private PostController BuildController(Mock<IPostRepository> postRepoMock)
    {
        var getAllHandler = new GetAllPostsQueryHandler(postRepoMock.Object);
        var getByIdHandler = new GetPostByIdQueryHandler(postRepoMock.Object);
        var getByUserIdHandler = new GetPostsByUserIdQueryHandler(postRepoMock.Object);
        var createHandler = new CreatePostCommandHandler(postRepoMock.Object);
        var updateHandler = new UpdatePostCommandHandler(postRepoMock.Object);
        var deleteHandler = new DeletePostCommandHandler(postRepoMock.Object);

        return new PostController(getAllHandler, getByIdHandler, getByUserIdHandler, createHandler, updateHandler, deleteHandler);
    }

    [Test]
    public async Task DeletePost_ShouldReturnOkWithMessage()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.DeletePostAsync("post-1")).Returns(Task.CompletedTask);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.DeletePost("post-1");

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
        mockRepo.Verify(r => r.DeletePostAsync("post-1"), Times.Once);
    }
}
