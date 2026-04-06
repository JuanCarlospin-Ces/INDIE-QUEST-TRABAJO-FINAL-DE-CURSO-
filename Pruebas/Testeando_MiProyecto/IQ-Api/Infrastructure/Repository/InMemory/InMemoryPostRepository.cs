using System;
using IQ_Api.Domain.Model;
using IQ_Api.Domain.Repository;

namespace IQ_Api.Infrastructure.Repository.InMemory;

public class InMemoryPostRepository : IPostRepository
{
    public List<Post> Posts { get; set; } = new()
    {
        new Post
        {
            PostId = Guid.NewGuid().ToString(),
            PostUserId = "userId1",
            Title = "First Post",
            MediaContent = "https://example.com/image1.jpg",
            Description = "This is the first post.",
            CreationDate = DateTime.UtcNow
        },
        new Post
        {
            PostId = Guid.NewGuid().ToString(),
            PostUserId = "userId2",
            Title = "Second Post",
            MediaContent = "https://example.com/image2.jpg",
            Description = "This is the second post.",
            CreationDate = DateTime.UtcNow
        },
        new Post
        {
            PostId = Guid.NewGuid().ToString(),
            PostUserId = "userId3",
            Title = "Third Post",
            MediaContent = "https://example.com/image3.jpg",
            Description = "This is the third post.",
            CreationDate = DateTime.UtcNow
        }
    };

    public Task<List<Post>> GetAllPostsAsync()
    {
        return Task.FromResult(Posts);
    }

    public Task<Post?> GetPostByIdAsync(string postId)
    {
        var post = Posts.FirstOrDefault(p => p.PostId == postId);
        return Task.FromResult(post);
    }

    public Task<List<Post>> GetPostsByUserIdAsync(string userId)
    {
        var userPosts = Posts.Where(p => p.PostUserId == userId).ToList();
        return Task.FromResult(userPosts);
    }

    public Task CreatePostAsync(Post post)
    {
        Posts.Add(post);
        return Task.CompletedTask;
    }

    public Task UpdatePostAsync(Post post)
    {
        var existingPost = Posts.FirstOrDefault(p => p.PostId == post.PostId);
        if (existingPost != null)
        {
            existingPost.Title = post.Title;
            existingPost.MediaContent = post.MediaContent;
            existingPost.Description = post.Description;
            existingPost.Tags = post.Tags;
        }
        return Task.CompletedTask;
    }

    public Task DeletePostAsync(string postId)
    {
        var post = Posts.FirstOrDefault(p => p.PostId == postId);
        if (post != null)
        {
            Posts.Remove(post);
        }
        return Task.CompletedTask;
    }
}

