using NUnit.Framework;
using OpenLibrary.Domain.Service;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Model;
using OpenLibrary.Infrastructure.Repository.InMemory;
using OpenLibrary.Domain.Exceptions;

namespace UnitTest;

public class FindBookByISBNTest
{
    [SetUp]
    public void Setup()
    { 
    }

    [Test]
    public void GetBookByISBNShouldReturnBook()
    {   
        InMemoryBookRepository bookRepository = new InMemoryBookRepository();
        bookRepository.InitializeBooks(new List<Book>
        {
            new Book
            {
                ISBN = new ISBN("978-3-16-148410-0"),
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                PublicationYear = 1925,
                Sales = 25000000
            },
            new Book
            {
                ISBN = new ISBN("978-0-14-118263-6"),
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                PublicationYear = 1960,
                Sales = 30000000
            }
        });

        ISBN isbnToLook = new ISBN("978-3-16-148410-0");
        Assert.That(GetBookByISBN.FindBookByISBN(isbnToLook, bookRepository).ISBN.ToString(), Is.EqualTo(isbnToLook.ToString()));
    }

    [Test]
    public void getBookByISBNShouldThrowExceptionIfNotFound()
    {
        InMemoryBookRepository bookRepository = new InMemoryBookRepository();
        bookRepository.InitializeBooks(new List<Book>
        {
            new Book
            {
                ISBN = new ISBN("978-3-16-148410-0"),
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                PublicationYear = 1925,
                Sales = 25000000
            },
            new Book
            {
                ISBN = new ISBN("978-0-14-118263-6"),
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                PublicationYear = 1960,
                Sales = 30000000
            }
        });

        ISBN isbnToLook = new ISBN("978-1-4028-9462-6");

        Assert.Throws<BookNotFoundException>(() => GetBookByISBN.FindBookByISBN(isbnToLook, bookRepository));
    }
}
