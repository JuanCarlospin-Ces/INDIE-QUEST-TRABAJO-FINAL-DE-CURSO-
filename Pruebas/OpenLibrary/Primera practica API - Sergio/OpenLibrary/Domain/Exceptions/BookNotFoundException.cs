using System;

namespace OpenLibrary.Domain.Exceptions;

public class BookNotFoundException : Exception
{
    public BookNotFoundException(string ISBN) : base($"No book found with ISBN: {ISBN}")
    {    }
}
