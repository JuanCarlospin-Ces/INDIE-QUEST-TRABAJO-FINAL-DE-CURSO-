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

public class UserControllerGetAllUsersTests
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
    public async Task GetAllUsers_ShouldReturnOkWithUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = "user-1", Username = "alice", Password = "pass", Email = "alice@example.com" },
            new User { UserId = "user-2", Username = "bob", Password = "pass", Email = "bob@example.com" }
        };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetAllUsers();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<List<User>>());
        var returnedUsers = (List<User>)okResult.Value!;
        Assert.That(returnedUsers.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllUsers_ShouldReturnOkWithEmptyList_WhenNoUsers()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());

        var controller = BuildController(mockRepo);

        // Act
        var result = await controller.GetAllUsers();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.InstanceOf<List<User>>());
        var returnedUsers = (List<User>)okResult.Value!;
        Assert.That(returnedUsers, Is.Empty);
    }
}
