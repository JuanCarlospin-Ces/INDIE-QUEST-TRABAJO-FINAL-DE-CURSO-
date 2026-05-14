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

public class PostControllerCreatePostTests
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
    public async Task CreatePost_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        mockRepo.Setup(r => r.CreatePostAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

        var controller = BuildController(mockRepo);

        var command = new CreatePostCommand
        {
            PostId = "post-1",
            PostUserId = "user-1",
            Title = "New Post",
            MediaContent = "https://example.com/image.jpg"
        };

        // Act
        var result = await controller.CreatePost(command);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
        mockRepo.Verify(r => r.CreatePostAsync(It.IsAny<Post>()), Times.Once);
    }
}
