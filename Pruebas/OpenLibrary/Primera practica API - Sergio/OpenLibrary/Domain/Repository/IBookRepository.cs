using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;


namespace OpenLibrary.Domain.Repository;

public interface IBookRepository
{
    public Task<List<Book>> GetAllBooksAsync();
    public Task<Book?> GetMostSoldBookAsync();
    public Task<Book?> GetBookByISBN(ISBN ISBN);
    public Task InsertBook(Book book);
    public Task DeleteBook(ISBN ISBN);
    public Task UpdateBook(Book book);
}
