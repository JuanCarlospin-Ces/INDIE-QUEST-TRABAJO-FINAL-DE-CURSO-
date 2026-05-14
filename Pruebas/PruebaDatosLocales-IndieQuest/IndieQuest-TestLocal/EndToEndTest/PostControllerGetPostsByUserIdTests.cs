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

public class PostControllerGetPostsByUserIdTests
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
    public async Task GetPostsByUserId_ShouldReturnOkWithPosts_WhenUserHasPosts()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post { PostId = "post-1", PostUserId = "user-1", Title = "Post 1", MediaContent = "url1", CreationDate = DateTime.UtcNow },
            new Post { PostId = "post-2", PostUserId = "user-1", Title = "Post 2", MediaContent = "url2", CreationDate = DateTime.UtcNow }
        };

        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostsByUserIdAsync("user-1")).ReturnsAsync(posts);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetPostsByUserId("user-1");

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<List<Post>>());
        var returnedPosts = (List<Post>)okResult.Value!;
        Assert.That(returnedPosts.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetPostsByUserId_ShouldReturnOkWithEmptyList_WhenUserHasNoPosts()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.GetPostsByUserIdAsync("user-99")).ReturnsAsync(new List<Post>());

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetPostsByUserId("user-99");

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<List<Post>>());
        var returnedPosts = (List<Post>)okResult.Value!;
        Assert.That(returnedPosts, Is.Empty);
    }
}
