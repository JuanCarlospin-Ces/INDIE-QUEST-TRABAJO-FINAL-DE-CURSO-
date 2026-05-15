using System;
using System.IO;
using System.Linq;
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
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? availableForWork = null)
    {
        var result = await _getAllUsersQueryHandler.Handle(pageNumber, pageSize, availableForWork);
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

        // Get the parent directory (project root) and create the user folder
        var projectRoot = Path.Combine(Directory.GetCurrentDirectory(), "..");
        var userFolder = Path.Combine(projectRoot, "IndieQuest-LocalData", "user", id.ToString());
        Directory.CreateDirectory(userFolder);

        // Delete all existing files in the folder (keep only the new image)
        foreach (var existingFile in Directory.GetFiles(userFolder))
            System.IO.File.Delete(existingFile);

        var safeFileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine(userFolder, safeFileName);
        
        // Store the relative path in the database
        var dbPath = $"IndieQuest-LocalData/user/{id}/{safeFileName}";

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
            UserProfilePicture = dbPath,
        });

        return Ok(new { path = dbPath });
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            return BadRequest(new { message = "Username and password are required." });
        }

        var allUsersResult = await _getAllUsersQueryHandler.Handle(1, 9999);
        var user = allUsersResult.Data.FirstOrDefault(u =>
            u.Username.Equals(loginRequest.Username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == loginRequest.Password);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(new
        {
            userId = user.UserId,
            username = user.Username,
            email = user.Email,
            userBio = user.UserBio,
            availableForWork = user.AvailableForWork,
            userProfilePicture = user.UserProfilePicture
        });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

