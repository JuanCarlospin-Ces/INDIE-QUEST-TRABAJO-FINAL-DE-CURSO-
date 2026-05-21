using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace IndieQuest_Api.Infrastructure.Repository.PostgreSQL;

public class PostgreSqlUserRepository : IUserRepository
{
    private readonly IndieQuestDbContext _context;

    public PostgreSqlUserRepository(IndieQuestDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<(List<User> Items, int TotalCount)> GetAllUsersPagedAsync(int pageNumber, int pageSize, bool? availableForWork = null)
    {
        IQueryable<User> query = _context.Users;

        if (availableForWork.HasValue)
        {
            query = query.Where(u => u.AvailableForWork == availableForWork.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.UserId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.UserPosts)
            .ThenInclude(up => up.Post)
            .FirstOrDefaultAsync(u => u.UserId == userId);
        
        if (user != null)
        {
            // Delete posts that only belong to this user and their media content
            foreach (var userPost in user.UserPosts.ToList())
            {
                var postUserCount = await _context.UserPosts
                    .Where(up => up.PostId == userPost.PostId)
                    .CountAsync();
                
                // If this post only has this user, delete the post and its media
                if (postUserCount == 1)
                {
                    // Delete post media folder
                    DeletePostMediaFolder(userPost.Post.PostId);
                    
                    // Delete the post (cascading will remove UserPost and PostTag)
                    _context.Posts.Remove(userPost.Post);
                }
                // If post has multiple users, just remove the UserPost relationship
                else
                {
                    _context.UserPosts.Remove(userPost);
                }
            }
            
            // Delete user profile picture folder
            DeleteUserProfileFolder(userId);
            
            // Delete the user (cascading will remove any remaining UserPost relationships)
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    private void DeleteUserProfileFolder(int userId)
    {
        try
        {
            var projectRoot = Path.Combine(Directory.GetCurrentDirectory(), "..");
            var userFolder = Path.Combine(projectRoot, "IndieQuest-LocalData", "user", userId.ToString());
            
            if (Directory.Exists(userFolder))
            {
                Directory.Delete(userFolder, true);
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the deletion if file deletion fails
            Console.WriteLine($"Warning: Could not delete user profile folder for userId {userId}: {ex.Message}");
        }
    }

    private void DeletePostMediaFolder(int postId)
    {
        try
        {
            var projectRoot = Path.Combine(Directory.GetCurrentDirectory(), "..");
            var postFolder = Path.Combine(projectRoot, "IndieQuest-LocalData", "postdata", postId.ToString());
            
            if (Directory.Exists(postFolder))
            {
                Directory.Delete(postFolder, true);
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the deletion if file deletion fails
            Console.WriteLine($"Warning: Could not delete post media folder for postId {postId}: {ex.Message}");
        }
    }
}
