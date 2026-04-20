using System;
using IndieQuest_Api.Domain.ValueObject;

namespace IndieQuest_Api.Domain.Model;

public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool? AvailableForWork { get; set; }
    public string? UserBio { get; set; }
    public string? UserProfilePicture { get; set; }
    public required string Email { get; set; }
    public DateTime dateOfRegistration { get; set; }
    
    // Propiedades de navegación
    public ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();
}
