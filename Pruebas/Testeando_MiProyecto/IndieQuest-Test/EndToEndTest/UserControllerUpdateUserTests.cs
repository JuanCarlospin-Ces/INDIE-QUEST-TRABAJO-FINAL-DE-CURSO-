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

public class UserControllerUpdateUserTests
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
    public async Task UpdateUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var existingUser = new User { UserId = "user-1", Username = "old_name", Password = "oldpass", Email = "old@example.com", dateOfRegistration = DateTime.UtcNow };

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("user-1")).ReturnsAsync(existingUser);
        mockRepo.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var controller = BuildController(mockRepo);

        var command = new UpdateUserCommand
        {
            UserId = "user-1",
            Username = "new_name",
            Password = "newpass",
            Email = "new@example.com"
        };

        // Act
        var result = await controller.UpdateUser("user-1", command);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
    }

    [Test]
    public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserByIdAsync("nonexistent")).ReturnsAsync((User?)null);

        var controller = BuildController(mockRepo);

        var command = new UpdateUserCommand
        {
            UserId = "nonexistent",
            Username = "name",
            Password = "pass",
            Email = "email@example.com"
        };

        // Act
        var result = await controller.UpdateUser("nonexistent", command);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
