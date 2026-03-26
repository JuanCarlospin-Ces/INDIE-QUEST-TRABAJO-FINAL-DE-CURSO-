using Microsoft.AspNetCore.Mvc;
using OpenLibrary.Application.Exceptions;
using OpenLibrary.Application.Queries.Books.GetAllBooks;
using OpenLibrary.Application.Queries.Books.GetMostSoldBook;
using OpenLibrary.Domain.Repository;
using OpenLibrary.Application.Command.Books;
using OpenLibrary.Controllers.dto;
using OpenLibrary.Domain.ValueObject;
using OpenLibrary.Domain.Service;

namespace OpenLibrary.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookController : ControllerBase
	{
		private readonly IBookRepository _bookRepository;

		// Constructor
		public BookController(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllBooks()
		{
			var queryHandler = new GetAllBooksQueryHandler(bookRepository: _bookRepository);
			var books = await queryHandler.Handle();

			return Ok(books);
		}

		[HttpGet]
		[Route("most-sold")]
		public async Task<IActionResult> GetMostSoldBooksAsync()
		{
			var queryHandler = new GetMostSoldBookQueryHandler(bookRepository: _bookRepository);
			try
			{
				var books = await queryHandler.Handle();
				return Ok(books);
			}
			catch (NotFoundException ex)
			{
				return NotFound($"Error retrieving most sold book: {ex.Message}");
			}

            
		}
		[HttpPost]
		public async Task<IActionResult> AddBook(BookDTO BookDTO) {
        
            try
            {
                var commandHandler = new AddBookCommandHandler(bookRepository: _bookRepository);
                await commandHandler.Handle(
                    new AddBookCommand
                    {
                        ISBN = new Domain.ValueObject.ISBN(BookDTO.ISBN),
                        Title = BookDTO.Title,
                        Author = BookDTO.Author,
                        PublicationYear = BookDTO.PublicationYear,
                        Sales = BookDTO.Sales
                    }
                );

                return Created();
            }
            catch (BookAlreadyExistsException ex)
            {
                
                return BadRequest(ex.Message);
            }
			
		}

		[HttpDelete]
		[Route("{isbn}")]
		public async Task<ActionResult> DeleteBook(string isbn)
		{
			try{
				var commandHandler = new DeleteBookCommandHandler(bookRepository: _bookRepository);
				await commandHandler.Handle(
					new DeleteBookCommand { 
						ISBN = new ISBN(isbn)
					});

				return NoContent();
			}
			catch (NotFoundException ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpPut]
		[Route("{isbn}")]
		public async Task<ActionResult> UpdateBook(BookDTO bookDTO)
		{
			try{
				var commandHandler = new UpdateBookCommandHandler(bookRepository: _bookRepository);
				await commandHandler.Handle(
					new UpdateBookCommand
					{
						ISBN = new ISBN(bookDTO.ISBN),
						Title = bookDTO.Title,
						Author = bookDTO.Author,
						PublicationYear = bookDTO.PublicationYear,
						Sales = bookDTO.Sales
					}
					
				);

				return NoContent();}
			catch (NotFoundException ex){
					return NotFound(ex.Message);
			}

		}
	}
}
