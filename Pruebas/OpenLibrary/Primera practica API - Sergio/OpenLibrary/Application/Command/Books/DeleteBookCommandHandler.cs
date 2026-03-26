using System;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Domain.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using OpenLibrary.Application.Exceptions;

namespace OpenLibrary.Application.Command.Books;

public class DeleteBookCommandHandler
{
    private readonly IBookRepository _bookRepository;

    public DeleteBookCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task Handle(DeleteBookCommand command)
    {
        var existingBook = await _bookRepository.GetBookByISBN(command.ISBN);
        if(existingBook == null)
        {
            throw new NotFoundException();
        }

        await _bookRepository.DeleteBook(command.ISBN);
    }
}
