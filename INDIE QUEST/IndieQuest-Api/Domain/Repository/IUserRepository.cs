using System;
using IndieQuest_Api.Domain.Model;

namespace IndieQuest_Api.Domain.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<(List<User> Items, int TotalCount)> GetAllUsersPagedAsync(int pageNumber, int pageSize);
    Task<User?> GetUserByIdAsync(int userId);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
}
