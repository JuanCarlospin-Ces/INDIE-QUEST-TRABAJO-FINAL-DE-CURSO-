using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Service;
using OpenLibrary.Infrastructure.Repository.InMemory;

namespace OpenLibrary.Infrastructure.Repository.InMemory;

public class InMemoryBookRepository : IBookRepository
{  
    private List<Book> _books = new List<Book> {
        new Book {
            ISBN = new ISBN("978-3-16-148410-0"),
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            PublicationYear = 1925,
            Sales = 45
        },
        new Book{
            ISBN = new ISBN("978-0-14-028333-4"),
            Title = "To Kill a Mockingbird",
            Author = "Harper Lee",
            PublicationYear = 1960,
            Sales = 69
        },
        new Book{
            ISBN = new ISBN("978-84-1107-748-4"),
            Title = "Warhammer 40.000 : Dark Imperium",
            Author = "Guy Haley",
            PublicationYear = 2017,
            Sales = 3576
        },
        new Book{
            ISBN = new ISBN("978-0-452-28423-4"),
            Title = "1984",
            Author = "George Orwell",
            PublicationYear = 1949,
            Sales = 25
        },
        new Book{
            ISBN = new ISBN("978-0-7432-7356-5"),
            Title = "The Da Vinci Code",
            Author = "Dan Brown",
            PublicationYear = 2003,
            Sales = 80
        },
        new Book{
            ISBN = new ISBN("978-0-316-76948-8"),
            Title = "The Catcher in the Rye",
            Author = "J.D. Salinger",
            PublicationYear = 1951,
            Sales = 65
        },
        new Book{
            ISBN = new ISBN("978-84-1121-425-4"),
            Title = "The lord of the rings",
            Author = "J.R.R. Tolkien",
            PublicationYear = 1954,
            Sales = 150
        },
        new Book{
            ISBN = new ISBN("978-84-1094-253-0"),
            Title = "Bof, Impresoras!",
            Author = "La de sistemas informáticos",
            PublicationYear = 2025,
            Sales = 50
        }

    };

    public Task<List<Book>> GetAllBooksAsync()
    {
        
        return Task.FromResult(_books);

    }

    public Task<Book?> GetMostSoldBookAsync()
    {
        return Task.FromResult(_books.OrderByDescending(b => b.Sales).FirstOrDefault());
    }

    public void InitializeBooks(List<Book> books)
    {
        _books = books;

    }

    public async Task InsertBook(Book book)
    {
        _books.Add(book);
    }

    public async Task<Book> GetBookByISBN(ISBN ISBN)
    {
        return _books.FirstOrDefault(b => b.ISBN.ToString() == ISBN.ToString());
    }

    public async Task DeleteBook(ISBN ISBN)
    {
        var bookToDelete = _books.FirstOrDefault(b => b.ISBN.ToString() == ISBN.ToString());
        if (bookToDelete != null)
        {
            _books.Remove(bookToDelete);
        }
    }
    
    public async Task UpdateBook(Book book)
    {
        var existingBook = _books.FirstOrDefault(b => b.ISBN.ToString() == book.ISBN.ToString());
        if (existingBook != null)
        {
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.PublicationYear = book.PublicationYear;
            existingBook.Sales = book.Sales;
        }
    }
}
