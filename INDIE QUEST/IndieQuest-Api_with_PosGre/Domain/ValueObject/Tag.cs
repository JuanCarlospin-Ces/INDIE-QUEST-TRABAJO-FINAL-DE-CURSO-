using System;

namespace IndieQuest_Api.Domain.ValueObject;

public class Tag
{
    public int tagId { get; set; }
    public string tagName { get; set; }    
    // Propiedades de navegación
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();}
