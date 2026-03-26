using NUnit.Framework;
using OpenLibrary.Controllers;
using OpenLibrary.Application.Command.Books;
using OpenLibrary.Infrastructure.Repository.InMemory;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Repository;
using Moq;
namespace AcceptanceTest;

public class UpdateBookCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void UpdateBookShouldUpdateBookSuccessfully()
    {
        Mock<IBookRepository> mockRepository = new Mock<IBookRepository>();

        var commandHandler = new UpdateBookCommandHandler(mockRepository.Object);
        var existingBook = new Book
        {
            ISBN = new ISBN("978-3-16-148410-0"),
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            PublicationYear = 1925,
            Sales = 25000000
        };

        var updatedBook = new UpdateBookCommand
        {
            ISBN = existingBook.ISBN,
            Title = "The Great Gatsby - Updated",
            Author = "F. Scott Fitzgerald",
            PublicationYear = 1925,
            Sales = 30000000
        };

        mockRepository.Setup(repo => repo.GetBookByISBN(existingBook.ISBN)).ReturnsAsync(existingBook);
        commandHandler.Handle(updatedBook);
        mockRepository.Verify(repo => repo.UpdateBook(It.Is<Book>(book => book.ISBN == updatedBook.ISBN && 
            book.Title == updatedBook.Title && 
            book.Author == updatedBook.Author && 
            book.PublicationYear == updatedBook.PublicationYear && 
            book.Sales == updatedBook.Sales
            )
        ), Times.Once);
    }
}
