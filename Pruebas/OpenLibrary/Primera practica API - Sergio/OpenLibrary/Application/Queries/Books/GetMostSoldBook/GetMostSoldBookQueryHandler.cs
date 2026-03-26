using System;
using OpenLibrary.Domain.Repository;    
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Application.Exceptions;

namespace OpenLibrary.Application.Queries.Books.GetMostSoldBook;

public class GetMostSoldBookQueryHandler
{
    private readonly IBookRepository _bookRepository;

    public GetMostSoldBookQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Book> Handle()
    {
        
        Book book = await _bookRepository.GetMostSoldBookAsync();
        if (book == null) {
            throw new NotFoundException();
        }
        return book;
    }
}
