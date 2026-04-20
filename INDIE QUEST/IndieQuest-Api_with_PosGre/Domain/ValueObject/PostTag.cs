using System;
using IndieQuest_Api.Domain.Model;

namespace IndieQuest_Api.Domain.ValueObject;

public class PostTag
{
    public int PostId { get; set; }
    public int TagId { get; set; }
    
    // Propiedades de navegación
    public Post Post { get; set; }
    public Tag Tag { get; set; }
}
