using System;

namespace OpenLibrary.Application.Exceptions;

public class BookAlreadyExistsException : Exception
{
    public BookAlreadyExistsException(string ISBN) : base(
        $"A book with the ISBN '{ISBN}' already exists in the library."
        ){}
}
