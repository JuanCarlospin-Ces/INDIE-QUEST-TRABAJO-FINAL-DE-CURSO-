using System;
using IndieQuest_Api.Domain.Model;

namespace IndieQuest_Api.Domain.Repository;

public interface IPostRepository
{
    Task<List<Post>> GetAllPostsAsync();
    Task<Post?> GetPostByIdAsync(int postId);
    Task<List<Post>> GetPostsByUserIdAsync(int userId);
    Task CreatePostAsync(Post post);
    Task UpdatePostAsync(Post post);
    Task DeletePostAsync(int postId);
}
