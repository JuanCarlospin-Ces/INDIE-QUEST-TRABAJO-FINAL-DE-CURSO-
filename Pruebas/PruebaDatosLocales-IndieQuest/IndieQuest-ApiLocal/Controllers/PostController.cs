using System;
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
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _getAllPostsQueryHandler.Handle();
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(string id)
    {
        var post = await _getPostByIdQueryHandler.Handle(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPostsByUserId(string userId)
    {
        var posts = await _getPostsByUserIdQueryHandler.Handle(userId);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
    {
        await _createPostCommandHandler.Handle(command);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(string id, [FromBody] UpdatePostCommand command)
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
    public async Task<IActionResult> DeletePost(string id)
    {
        await _deletePostCommandHandler.Handle(id);
        return Ok(new { message = "Post deleted successfully" });
    }
}



