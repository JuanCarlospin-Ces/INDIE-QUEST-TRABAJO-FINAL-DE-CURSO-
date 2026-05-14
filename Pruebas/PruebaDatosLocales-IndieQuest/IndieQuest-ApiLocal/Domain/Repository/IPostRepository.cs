using System;
using IndieQuest_Api.Domain.Model;

namespace IndieQuest_Api.Domain.Repository;

public interface IPostRepository
{
    Task<List<Post>> GetAllPostsAsync();
    Task<Post?> GetPostByIdAsync(string postId);
    Task<List<Post>> GetPostsByUserIdAsync(string userId);
    Task CreatePostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(string postId);
}
