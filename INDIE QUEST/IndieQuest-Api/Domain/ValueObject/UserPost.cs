using System;
using IndieQuest_Api.Domain.Model;

namespace IndieQuest_Api.Domain.ValueObject;

public class UserPost
{
    public int UserId { get; set; }
    public int PostId { get; set; }
    
    // Propiedades de navegación
    public User User { get; set; }
    public Post Post { get; set; }
}
