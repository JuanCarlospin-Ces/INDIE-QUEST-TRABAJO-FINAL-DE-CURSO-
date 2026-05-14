using System;
using IndieQuest_Api.Domain.ValueObject;

namespace IndieQuest_Api.Domain.Model;

public class Post
{
    public int PostId { get; set; }
    public required string Title { get; set; }
    public required string MediaContent { get; set; }
    
    public string? Description { get; set; }
    public DateTime CreationDate { get; set; }

    // Calculado: primer userId de la relación Makes_MadeBy — lo serializa la API
    // para que el frontend no tenga que navegar por userPosts[0].userId
    public int? PostUserId => UserPosts?.FirstOrDefault()?.UserId;

    // Propiedades de navegación
    public ICollection<UserPost> UserPosts { get; set; } = new List<UserPost>();
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
