using FD.TechTest.LibraryManagement.Domain.Entities;
using FD.TechTest.LibraryManagement.Domain.Exceptions;
using FD.TechTest.LibraryManagement.Domain.Repositories;

namespace FD.TechTest.LibraryManagement.Domain.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICustomerRepository _customerRepository;

        public LibraryService(IBookRepository bookRepository, ICustomerRepository customerRepository)
        {
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Récupère tous les livres de la bibliothèque
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public IReadOnlyCollection<Customer> GetCustomersWhoBorrowedBooks()
        {
            return _customerRepository
                .GetAll()
                .Where(customer => customer.BorrowedBooks?.Any() == true)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Ajoute un livre à la bibliothèque
        /// </summary>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <returns>L'identifiant du livre ajouté</returns>
        public int AddBook(string title, string author)
        {
            var normalizedTitle = title?.Trim();
            var normalizedAuthor = author?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedTitle) || normalizedTitle.Length < 3)
            {
                throw new LibraryManagementException("Title must contain at least 3 characters.");
            }

            if (string.IsNullOrWhiteSpace(normalizedAuthor))
            {
                throw new LibraryManagementException("Author is required.");
            }

            var allBooks = _bookRepository.GetAll();
            if (allBooks.Any(book =>
                    string.Equals(book.Title, normalizedTitle, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(book.Author, normalizedAuthor, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateBookException("A book with the same title and author already exists.");
            }

            var id = allBooks.Count == 0 ? 1 : allBooks.Max(b => b.Id) + 1;

            var book = new Book(id, normalizedTitle, true, normalizedAuthor);

            _bookRepository.AddBook(book);

            return book.Id;
        }
    }
}
