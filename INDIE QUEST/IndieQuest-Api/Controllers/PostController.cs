using System;
using System.IO;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Application.Queries.GetAllPosts;
using IndieQuest_Api.Application.Queries.GetPostById;
using IndieQuest_Api.Application.Queries.GetPostsByUserId;
using Microsoft.AspNetCore.Mvc;

namespace IndieQuest_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly GetAllPostsQueryHandler _getAllPostsQueryHandler;
    private readonly GetPostByIdQueryHandler _getPostByIdQueryHandler;
    private readonly GetPostsByUserIdQueryHandler _getPostsByUserIdQueryHandler;
    private readonly CreatePostCommandHandler _createPostCommandHandler;
    private readonly UpdatePostCommandHandler _updatePostCommandHandler;
    private readonly DeletePostCommandHandler _deletePostCommandHandler;

    public PostController(
        GetAllPostsQueryHandler getAllPostsQueryHandler,
        GetPostByIdQueryHandler getPostByIdQueryHandler,
        GetPostsByUserIdQueryHandler getPostsByUserIdQueryHandler,
        CreatePostCommandHandler createPostCommandHandler,
        UpdatePostCommandHandler updatePostCommandHandler,
        DeletePostCommandHandler deletePostCommandHandler)
    {
        _getAllPostsQueryHandler = getAllPostsQueryHandler;
        _getPostByIdQueryHandler = getPostByIdQueryHandler;
        _getPostsByUserIdQueryHandler = getPostsByUserIdQueryHandler;
        _createPostCommandHandler = createPostCommandHandler;
        _updatePostCommandHandler = updatePostCommandHandler;
        _deletePostCommandHandler = deletePostCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _getAllPostsQueryHandler.Handle(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _getPostByIdQueryHandler.Handle(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPostsByUserId(int userId)
    {
        var posts = await _getPostsByUserIdQueryHandler.Handle(userId);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
    {
        var post = await _createPostCommandHandler.Handle(command);
        return Ok(new { postId = post.PostId });
    }

    [HttpPost("{id}/media")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPostMedia(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        try
        {
            var post = await _getPostByIdQueryHandler.Handle(id);
            if (post == null) return NotFound("Post not found");

            // Get the parent directory (project root) and create the postdata folder
            var projectRoot = Path.Combine(Directory.GetCurrentDirectory(), "..");
            var postFolder = Path.Combine(projectRoot, "IndieQuest-LocalData", "postdata", id.ToString());
            
            // Ensure directory exists
            if (!Directory.Exists(postFolder))
                Directory.CreateDirectory(postFolder);

            // Delete all existing files in the folder (keep only the new media)
            var existingFiles = Directory.GetFiles(postFolder);
            foreach (var existingFile in existingFiles)
            {
                try
                {
                    System.IO.File.Delete(existingFile);
                }
                catch (Exception deleteEx)
                {
                    // Log but don't fail if can't delete old files
                    Console.WriteLine($"Could not delete old file {existingFile}: {deleteEx.Message}");
                }
            }

            var safeFileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(postFolder, safeFileName);
            
            // Store the relative path in the database
            var dbPath = $"IndieQuest-LocalData/postdata/{id}/{safeFileName}";

            // Copy file to disk
            using (var stream = System.IO.File.Create(filePath))
                await file.CopyToAsync(stream);

            var updateCommand = new UpdatePostCommand
            {
                PostId       = post.PostId,
                Title        = post.Title,
                MediaContent = dbPath,
                Description  = post.Description,
            };
            await _updatePostCommandHandler.Handle(updateCommand);

            return Ok(new { path = dbPath, message = "Media uploaded successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading media for post {id}: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, $"Error uploading file: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostCommand command)
    {
        command.PostId = id;
        var updatedPost = await _updatePostCommandHandler.Handle(command);
        if (updatedPost == null)
        {
            return NotFound();
        }
        return Ok(updatedPost);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        await _deletePostCommandHandler.Handle(id);
        return Ok(new { message = "Post deleted successfully" });
    }
}



