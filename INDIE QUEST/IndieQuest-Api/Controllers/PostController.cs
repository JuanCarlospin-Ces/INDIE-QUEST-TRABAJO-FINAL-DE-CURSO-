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

        var post = await _getPostByIdQueryHandler.Handle(id);
        if (post == null) return NotFound();

        var postFolder = $"IndieQuest-LocalData/postdata/{id}";
        Directory.CreateDirectory(postFolder);

        var safeFileName = Path.GetFileName(file.FileName);
        var filePath = $"{postFolder}/{safeFileName}";

        using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream);

        var updateCommand = new UpdatePostCommand
        {
            PostId       = post.PostId,
            Title        = post.Title,
            MediaContent = filePath,
            Description  = post.Description,
        };
        await _updatePostCommandHandler.Handle(updateCommand);

        return Ok(new { path = filePath });
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



