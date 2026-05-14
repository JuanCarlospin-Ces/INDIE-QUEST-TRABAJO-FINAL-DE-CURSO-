using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Api.Infrastructure.Repository.PostgreSQL;

public class PostgreSqlPostRepository : IPostRepository
{
    private readonly IndieQuestDbContext _context;

    public PostgreSqlPostRepository(IndieQuestDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _context.Posts
            .Include(p => p.UserPosts)
            .Include(p => p.PostTags)
            .ToListAsync();
    }

    public async Task<Post?> GetPostByIdAsync(int postId)
    {
        return await _context.Posts
            .Include(p => p.UserPosts)
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
    {
        return await _context.Posts
            .Where(p => p.UserPosts.Any(up => up.UserId == userId))
            .Include(p => p.UserPosts)
            .Include(p => p.PostTags)
            .ToListAsync();
    }

    public async Task CreatePostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        var existingPost = await _context.Posts
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.PostId == post.PostId);

        if (existingPost != null)
        {
            existingPost.Title = post.Title;
            existingPost.MediaContent = post.MediaContent;
            existingPost.Description = post.Description;
            
            // Actualizar PostTags si es necesario
            if (post.PostTags != null)
            {
                existingPost.PostTags = post.PostTags;
            }

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeletePostAsync(int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}
