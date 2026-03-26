using System;

namespace OpenLibrary.Controllers.dto;

public class BookDTO
{
    public string ISBN { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public int Sales { get; set; }
}
