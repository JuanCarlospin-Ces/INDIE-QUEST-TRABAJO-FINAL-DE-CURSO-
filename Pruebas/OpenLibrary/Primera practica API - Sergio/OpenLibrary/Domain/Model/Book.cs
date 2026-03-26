using System;
using OpenLibrary.Domain.ValueObject;

namespace OpenLibrary.Domain.Model;

public class Book
{
    public ISBN ISBN { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }

    public int Sales { get; set; } 


}