using System;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Application.Command.Books;
using OpenLibrary.Domain.Model;
using OpenLibrary.Application.Exceptions;

namespace OpenLibrary.Application.Command.Books;

public class UpdateBookCommandHandler
{
    private readonly IBookRepository _bookRepository;

    public UpdateBookCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task Handle(UpdateBookCommand command)
    {
        var existingBook = await _bookRepository.GetBookByISBN(command.ISBN);
        if(existingBook == null)
        {
            throw new NotFoundException();
        }
        existingBook.Title = command.Title;
        existingBook.Author = command.Author;
        existingBook.PublicationYear = command.PublicationYear;
        existingBook.Sales = command.Sales;

        await _bookRepository.UpdateBook(existingBook);
    }
}
