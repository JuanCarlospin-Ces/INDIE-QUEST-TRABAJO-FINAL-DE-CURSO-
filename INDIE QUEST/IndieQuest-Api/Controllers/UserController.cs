using System;
using System.IO;
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
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _getAllUsersQueryHandler.Handle(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
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
        var user = await _createUserCommandHandler.Handle(command);
        return Ok(new { userId = user.UserId });
    }

    [HttpPost("{id}/picture")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfilePicture(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var user = await _getUserByIdQueryHandler.Handle(id);
        if (user == null) return NotFound();

        var userFolder = $"IndieQuest-LocalData/user/{id}";
        Directory.CreateDirectory(userFolder);

        var safeFileName = Path.GetFileName(file.FileName);
        var filePath = $"{userFolder}/{safeFileName}";

        using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream);

        await _updateUserCommandHandler.Handle(new UpdateUserCommand
        {
            UserId             = user.UserId,
            Username           = user.Username,
            Password           = user.Password,
            Email              = user.Email,
            UserBio            = user.UserBio,
            AvailableForWork   = user.AvailableForWork,
            UserProfilePicture = filePath,
        });

        return Ok(new { path = filePath });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
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
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _deleteUserCommandHandler.Handle(id);
        return Ok(new { message = "User deleted successfully" });
    }
}

