using NUnit.Framework;
using OpenLibrary.Controllers;
using OpenLibrary.Application.Command.Books;
using OpenLibrary.Infrastructure.Repository.InMemory;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Repository;
using Moq;

namespace AcceptanceTest;

public class DeleteBookCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task DeleteBookThatExistsShouldDeleteBookSuccessfully()
    {
        Mock<IBookRepository> mockRepository = new Mock<IBookRepository>();

        var commandHandler = new DeleteBookCommandHandler(mockRepository.Object);
        var isbnToDelete = new ISBN("978-3-16-148410-0");

        // Arrange: repository returns an existing book for the ISBN
        var existingBook = new Book { ISBN = isbnToDelete, Title = "Title", Author = "Author", PublicationYear = 2000, Sales = 0 };
        mockRepository.Setup(r => r.GetBookByISBN(isbnToDelete)).ReturnsAsync(existingBook);
        mockRepository.Setup(r => r.DeleteBook(isbnToDelete)).Returns(Task.CompletedTask);

        // Act
        await commandHandler.Handle(new DeleteBookCommand { ISBN = isbnToDelete });

        // Assert
        mockRepository.Verify(repo => repo.DeleteBook(isbnToDelete), Times.Once);
    }

}
