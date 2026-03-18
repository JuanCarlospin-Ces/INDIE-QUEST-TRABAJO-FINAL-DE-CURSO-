using System;

namespace IQ_Api.Domain.Model;

public class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    public bool AvailableForWork { get; set; }

}
