using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenLibrary.Controllers;
using OpenLibrary.Infrastructure.Repository.InMemory;
using Microsoft.AspNetCore.Http;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Domain.Model;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Controllers.dto;

namespace EndToEndTest
{
    public class BookControllerTest
    {
        private BookController _controller;
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task GetMostSoldBooksShouldReturnMostSoldBooks()
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
            _controller = new BookController(bookRepository);
            
            var result = await _controller.GetMostSoldBooksAsync() as OkObjectResult;

            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        }
        [Test]
        public async Task GetMostSoldBooksShouldReturn404IfNoBooksFound() { 
            var bookRepository = new InMemoryBookRepository();
            bookRepository.InitializeBooks(new List<Book>());
            _controller = new BookController(bookRepository);

            var result = await _controller.GetMostSoldBooksAsync() as NotFoundObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }
        [Test]
        public async Task InsertNewBookThatDoesNotExistShouldReturn201Created()
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

            _controller = new BookController(bookRepository);
            var newBook = new BookDTO
            {
                ISBN = "978-1-56619-909-4",
                Title = "New Book",
                Author = "New Author",
                PublicationYear = 2020,
                Sales = 0
            };

            var result = await _controller.AddBook(newBook) as CreatedResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
             
        }
        
        
        [Test]
        public async Task InsertNewBookThatDoesExistShouldReturn400()
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

            _controller = new BookController(bookRepository);
            var newExistingBook = new BookDTO
            {
                ISBN = "978-3-16-148410-0",
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                PublicationYear = 1925,
                Sales = 25000000
            };

            var result = await _controller.AddBook(newExistingBook) as BadRequestObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

        }
        
        [Test]
        public async Task DeleteExistingBookThatExistsShouldReturn204NoContent()
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

            _controller = new BookController(bookRepository);
            var result = await _controller.DeleteBook(new ISBN("978-3-16-148410-0")) as NoContentResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));

        }

        
        [Test]
        public async Task DeleteBookThatDoesNotExistShouldReturn404()
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

            _controller = new BookController(bookRepository);
            var result = await _controller.DeleteBook(new ISBN("978-1-56619-909-4")) as NotFoundObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task UpdateExistingBookThatExistsShouldReturn204NoContent()
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

            _controller = new BookController(bookRepository);
            var updatedBookDTO = new BookDTO
            {
                ISBN = "978-3-16-148410-0",
                Title = "The Great Gatsby - Updated",
                Author = "F. Scott Fitzgerald",
                PublicationYear = 1925,
                Sales = 30000000
            };

            var result = await _controller.UpdateBook(new ISBN("978-3-16-148410-0"), updatedBookDTO) as NoContentResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
        }
        
        [Test]
        public async Task UpdateBookThatDoesNotExistShouldReturn404()
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

            _controller = new BookController(bookRepository);
            var updatedBookDTO = new BookDTO
            {
                ISBN = "978-1-56619-909-4",
                Title = "Non Existing Book",
                Author = "Unknown Author",
                PublicationYear = 2020,
                Sales = 0
            };

            var result = await _controller.UpdateBook(new ISBN("978-1-56619-909-4"), updatedBookDTO) as NotFoundObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }
    }
}
