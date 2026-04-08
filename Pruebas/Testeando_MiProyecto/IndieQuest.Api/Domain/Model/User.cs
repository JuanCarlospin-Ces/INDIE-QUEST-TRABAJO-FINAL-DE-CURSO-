using System;

namespace IQ_Api.Domain.Model;

public class User
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool? AvailableForWork { get; set; }
    public string? UserBio { get; set; }
    public string? UserProfilePicture { get; set; }
    public string Email { get; set; }
    public DateTime dateOfRegistration { get; set; }
}
