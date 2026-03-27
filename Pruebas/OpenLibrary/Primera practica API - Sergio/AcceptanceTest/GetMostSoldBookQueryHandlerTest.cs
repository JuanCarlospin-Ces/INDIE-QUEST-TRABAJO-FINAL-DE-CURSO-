using Moq;
using NUnit.Framework;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Application.Queries.Books.GetMostSoldBook;
using OpenLibrary.Application.Exceptions;

namespace AcceptanceTest;

public class GetMostSoldBookQueryHandlerTest
{
    GetMostSoldBookQueryHandler _handler;

    Mock<IBookRepository> _bookRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _handler = new GetMostSoldBookQueryHandler(_bookRepositoryMock.Object);
    }

    [Test]
    public async Task QueryHandler_ShouldReturnMostSoldBook()
    {
        _bookRepositoryMock.Setup(x => x.GetMostSoldBookAsync()).ReturnsAsync(new Book
        {
            Title = "Most Sold Book",
            Author = "Famous Author",
            ISBN = new ISBN("0-306-40615-2"),
            PublicationYear = 2020
        });

        var result = await _handler.Handle();
        Assert.That(result.Title, Is.EqualTo("Most Sold Book"));
        Assert.That(result.Author, Is.EqualTo("Famous Author"));
        Assert.That(result.ISBN.ToString(), Is.EqualTo("0-306-40615-2"));
        Assert.That(result.PublicationYear, Is.EqualTo(2020));
    }

    [Test]
    public async Task QueryHandler_ShouldReturnNotFoundExceptionIfNoBooksFound() 
    {
        _bookRepositoryMock.Setup(x => x.GetMostSoldBookAsync()).ReturnsAsync( (Book)null );

        var exception = Assert.ThrowsAsync<NotFoundException>(async () => await _handler.Handle());
        Assert.That(exception.Message, Is.EqualTo("No books found in the library."));
    }
}
