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

namespace IndieQuest_Test.EndToEndTest;

public class PostControllerGetPostByIdTests
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
    public async Task GetPostById_ShouldReturnOk_WhenPostExists()
    {
        // Arrange
        var post = new Post { PostId = "post-1", PostUserId = "user-1", Title = "Title", MediaContent = "url", CreationDate = DateTime.UtcNow };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync("post-1")).ReturnsAsync(post);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetPostById("post-1");

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<Post>());
        var returnedPost = (Post)okResult.Value!;
        Assert.That(returnedPost.PostId, Is.EqualTo("post-1"));
    }

    [Test]
    public async Task GetPostById_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostByIdAsync("nonexistent")).ReturnsAsync((Post?)null);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetPostById("nonexistent");

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
