using OpenLibrary.Infrastructure.Repository.InMemory;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;

namespace IntegrationTest
{
    public class InMemoryBookRepositoryTest
    {    
        private InMemoryBookRepository _repository;
        [SetUp]
        public void Setup()
        {
            _repository = new InMemoryBookRepository();
            _repository.InitializeBooks(new List<Book>
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
                Sales = 3
            },
            new Book
            {
                ISBN = new ISBN("978-0-452-28423-4"),
                Title = "1984",
                Author = "George Orwell",
                PublicationYear = 1949,
                Sales = 1
            },
            new Book
            {
                ISBN = new ISBN("978-0-7432-7356-5"),
                Title = "The Da Vinci Code",
                Author = "Dan Brown",
                PublicationYear = 2003,
                Sales = 2
            }
        });
        }

        [Test]
        public async Task GetMostSoldBookShouldReturnMostSoldBook()
        {
            var result = await _repository.GetMostSoldBookAsync();
                Assert.That(result.Title, Is.EqualTo("The Great Gatsby"));
                Assert.That(result.Author, Is.EqualTo("F. Scott Fitzgerald"));
                Assert.That(result.ISBN.Value, Is.EqualTo("978-3-16-148410-0"));
                Assert.That(result.PublicationYear, Is.EqualTo(1925));
        }

        [Test]
        public async Task InsertNewBookShouldAddBookToRepository()
        {
            var newBook = new Book
            {
                ISBN = new ISBN("978-1-56619-909-4"),
                Title = "New Book",
                Author = "New Author",
                PublicationYear = 2020,
                Sales = 0
            };

            _repository.InsertBook(newBook);

            var result = await _repository.GetAllBooksAsync();
            Assert.That(result.Count, Is.EqualTo(5));

            var insertedBook = result.FirstOrDefault(b => b.ISBN.Value == "978-1-56619-909-4");
            Assert.That(insertedBook, Is.Not.Null);

            Assert.That(insertedBook.Title, Is.EqualTo(newBook.Title));
            Assert.That(insertedBook.Author, Is.EqualTo(newBook.Author));
            Assert.That(insertedBook.PublicationYear, Is.EqualTo(newBook.PublicationYear));
            Assert.That(insertedBook.Sales, Is.EqualTo(newBook.Sales));

        }
       
    }
}
