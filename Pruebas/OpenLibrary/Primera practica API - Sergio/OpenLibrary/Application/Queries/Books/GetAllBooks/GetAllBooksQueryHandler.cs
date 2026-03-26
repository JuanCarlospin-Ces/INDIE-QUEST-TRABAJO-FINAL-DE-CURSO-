using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.Repository;

namespace OpenLibrary.Application.Queries.Books.GetAllBooks;


public class GetAllBooksQueryHandler
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<Book>> Handle()
    {
        return await _bookRepository.GetAllBooksAsync();
    }
}
