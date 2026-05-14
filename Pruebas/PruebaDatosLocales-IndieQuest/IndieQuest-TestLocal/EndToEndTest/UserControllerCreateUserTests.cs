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

public class UserControllerCreateUserTests
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
    public async Task CreateUser_ShouldReturnOk_WhenCommandIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var controller = BuildController(mockRepo);

        var command = new CreateUserCommand
        {
            UserId = "user-1",
            Username = "alice",
            Password = "password123",
            Email = "alice@example.com",
            AvailableForWork = true
        };

        // Act
        var result = await controller.CreateUser(command);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
        mockRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }
}
