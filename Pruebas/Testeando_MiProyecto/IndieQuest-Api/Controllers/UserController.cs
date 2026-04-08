using System;
using IndieQuest_Api.Application.Command.Users;
using IndieQuest_Api.Application.Queries.GetAllUsers;
using IndieQuest_Api.Application.Queries.GetUserById;
using Microsoft.AspNetCore.Mvc;

namespace IndieQuest_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly GetAllUsersQueryHandler _getAllUsersQueryHandler;
    private readonly GetUserByIdQueryHandler _getUserByIdQueryHandler;
    private readonly CreateUserCommandHandler _createUserCommandHandler;
    private readonly UpdateUserCommandHandler _updateUserCommandHandler;
    private readonly DeleteUserCommandHandler _deleteUserCommandHandler;

    public UserController(
        GetAllUsersQueryHandler getAllUsersQueryHandler,
        GetUserByIdQueryHandler getUserByIdQueryHandler,
        CreateUserCommandHandler createUserCommandHandler,
        UpdateUserCommandHandler updateUserCommandHandler,
        DeleteUserCommandHandler deleteUserCommandHandler)
    {
        _getAllUsersQueryHandler = getAllUsersQueryHandler;
        _getUserByIdQueryHandler = getUserByIdQueryHandler;
        _createUserCommandHandler = createUserCommandHandler;
        _updateUserCommandHandler = updateUserCommandHandler;
        _deleteUserCommandHandler = deleteUserCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _getAllUsersQueryHandler.Handle();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _getUserByIdQueryHandler.Handle(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        await _createUserCommandHandler.Handle(command);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
    {
        command.UserId = id;
        var updatedUser = await _updateUserCommandHandler.Handle(command);
        if (updatedUser == null)
        {
            return NotFound();
        }
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _deleteUserCommandHandler.Handle(id);
        return Ok(new { message = "User deleted successfully" });
    }
}

