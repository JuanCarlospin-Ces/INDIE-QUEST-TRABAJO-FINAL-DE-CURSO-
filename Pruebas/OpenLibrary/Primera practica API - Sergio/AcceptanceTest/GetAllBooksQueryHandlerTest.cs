using Moq;
using OpenLibrary.Application.Queries.Books.GetAllBooks;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace AcceptanceTest
{
    public class GetAllBooksQueryHandlerTest
    {
        GetAllBooksQueryHandler _handler;
        Mock<IBookRepository> mockBookRepository = new Mock<IBookRepository>();

        [SetUp]
        public void Setup()
        {
            _handler = new GetAllBooksQueryHandler(mockBookRepository.Object);
        }

        [Test]
        public async Task GetBooksQueryHandlerShouldReturnAListOfBooks() {
            mockBookRepository.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(new List<Book> {
                new Book { Title = "Book 1", Author = "Author 1", ISBN= new ISBN("0-306-40615-2") },
                new Book { Title = "Book 2", Author = "Author 2", ISBN = new ISBN("0-306-40615-2") }
            });
            List<Book> result = await _handler.Handle();

            Assert.That(result.Count, Is.EqualTo(2)) ;
        }
    }
}
