using FD.TechTest.LibraryManagement.Domain.Services;
using FD.TechTest.LibraryManagement.Domain.Exceptions;
using FD.TechTest.LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FD.TechTest.LibraryManagement.Controllers
{
    [Route("api/library")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _bookService;

        public LibraryController(ILibraryService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Récupère tous les livres de la bibliothèque
        /// </summary>
        /// <returns>Tous les livres</returns>
        [HttpGet("books")]
        public IActionResult GetAllBooks()
        {
            var books = _bookService.GetAllBooks();
            return Ok(books);
        }

        /// <summary>
        /// Récupère les clients ayant emprunté des livres.
        /// </summary>
        /// <returns>La liste des clients ayant emprunté au moins un livre</returns>
        [HttpGet("/api/customers-who-borrowed-books")]
        public IActionResult GetCustomersWhoBorrowedBooks()
        {
            var customers = _bookService.GetCustomersWhoBorrowedBooks();
            return Ok(customers.Select(customer => new CustomerResponse(
                customer.Id,
                customer.Name,
                customer.BorrowedBooks.Select(book => new BorrowedBookResponse(book.BookId, book.BorrowedAt)).ToList())));
        }

        /// <summary>
        /// Ajoute un livre à la bibliothèque
        /// </summary>
        /// <param name="request">Requête pour ajouter un livre</param>
        /// <returns>L'identifiant du livre ajouté</returns>
        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequest request)
        {
            try
            {
                var addedBookId = _bookService.AddBook(request.Title, request.Author);
                return Ok(addedBookId);
            }
            catch (DuplicateBookException exception)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Duplicate book",
                    Detail = exception.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (LibraryManagementException exception)
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Invalid book",
                    Detail = exception.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }
    }
}
