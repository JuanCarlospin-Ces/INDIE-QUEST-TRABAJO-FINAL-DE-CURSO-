using System;
using Microsoft.VisualBasic;
using OpenLibrary.Application.Exceptions;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.Repository;

namespace OpenLibrary.Application.Command.Books;

public class AddBookCommandHandler
{
    private readonly IBookRepository _bookRepository;

    public AddBookCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task Handle(AddBookCommand command)
    {
        var existingBook = await _bookRepository.GetBookByISBN(command.ISBN);
        if(existingBook != null)
        {
            throw new BookAlreadyExistsException(command.ISBN.ToString());
        }
        var book = new Book
        {
            ISBN = command.ISBN,
            Title = command.Title,
            Author = command.Author,
            PublicationYear = command.PublicationYear,
            Sales = command.Sales,
            CoverImageBase64 = command.CoverImageBase64
        };

        await _bookRepository.InsertBook(book);
    }

}
