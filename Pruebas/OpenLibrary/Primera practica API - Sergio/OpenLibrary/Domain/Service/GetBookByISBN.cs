using System;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Domain.Exceptions;
using OpenLibrary.Infrastructure.Repository.InMemory;

namespace OpenLibrary.Domain.Service;

public static class GetBookByISBN
{
    public static Book FindBookByISBN(ISBN isbn, IBookRepository bookRepository)
    {
        var book = bookRepository.GetBookByISBN(isbn).Result;
        if (book == null)
        {
            throw new BookNotFoundException(isbn.ToString());
        }
        return book;
    }

}
