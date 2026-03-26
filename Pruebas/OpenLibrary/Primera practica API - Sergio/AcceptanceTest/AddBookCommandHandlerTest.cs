using NUnit.Framework;
using OpenLibrary.Controllers;
using OpenLibrary.Application.Command.Books;
using OpenLibrary.Infrastructure.Repository.InMemory;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Repository;
using Moq;

namespace AcceptanceTest;

public class AddBookCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void AddBookShouldAddBookSuccessfully()
    {
        Mock<IBookRepository> mockRepository = new Mock<IBookRepository>();

        var commandHandler = new AddBookCommandHandler(mockRepository.Object);
        var newBook = new AddBookCommand
        {
            ISBN = new ISBN("978-3-16-148410-0"),
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            PublicationYear = 1925,
            Sales = 25000000
        };

        commandHandler.Handle(newBook);

        mockRepository.Verify(repo => repo.InsertBook(It.Is<Book>(book => book.ISBN == newBook.ISBN && 
            book.Title == newBook.Title && 
            book.Author == newBook.Author && 
            book.PublicationYear == newBook.PublicationYear && 
            book.Sales == newBook.Sales
            )
        ), Times.Once);
        
    }
}
