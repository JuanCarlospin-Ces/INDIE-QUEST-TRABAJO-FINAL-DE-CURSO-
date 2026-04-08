using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using IndieQuest_Api.Controllers;
using IndieQuest_Api.Application.Queries.GetAllUsers;
using IndieQuest_Api.Application.Queries.GetUserById;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;

namespace IndieQuest_Test.EndToEndTest;

public class UserControllerGetUserByIdTests
{
    [SetUp]
    public void Setup()
    {
    }

    private UserController BuildController(Mock<IUserRepository> userRepoMock)
    {
        var getAllHandler = new GetAllUsersQueryHandler(userRepoMock.Object);
        var getByIdHandler = new GetUserByIdQueryHandler(userRepoMock.Object);
        var createHandler = new CreateUserCommandHandler(userRepoMock.Object);
        var updateHandler = new UpdateUserCommandHandler(userRepoMock.Object);
        var deleteHandler = new DeleteUserCommandHandler(userRepoMock.Object);

        return new UserController(getAllHandler, getByIdHandler, createHandler, updateHandler, deleteHandler);
    }

    [Test]
    public async Task GetUserById_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var user = new User { UserId = "user-1", Username = "alice", Password = "pass", Email = "alice@example.com" };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("user-1")).ReturnsAsync(user);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetUserById("user-1");

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<User>());
        var returnedUser = (User)okResult.Value!;
        Assert.That(returnedUser.UserId, Is.EqualTo("user-1"));
    }

    [Test]
    public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetUserById("nonexistent");

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
