using FD.TechTest.LibraryManagement.Domain.Entities;
using FD.TechTest.LibraryManagement.Domain.Exceptions;
using FD.TechTest.LibraryManagement.Domain.Repositories;
using FD.TechTest.LibraryManagement.Domain.Services;

namespace FD.TechTest.LibraryMangement.ApplicationCore.Tests;

public class LibraryServiceTests
{
    [Test]
    public void GetCustomersWhoBorrowedBooks_ShouldReturnOnlyCustomersWithBorrowedBooks()
    {
        var customersWithBorrowedBooks = new Customer[]
        {
            new(1, "Borrowing customer", new[] { new BorrowedBook(1, new DateTime(2026, 1, 1)) }),
            new(2, "Non-borrowing customer", Array.Empty<BorrowedBook>()),
            new(3, "Another borrowing customer", new[] { new BorrowedBook(2, new DateTime(2026, 1, 2)) })
        };
        var service = CreateService(customers: customersWithBorrowedBooks);

        var customers = service.GetCustomersWhoBorrowedBooks();

        Assert.That(customers.Select(customer => customer.Id), Is.EqualTo(new[] { 1, 3 }));
    }

    [Test]
    public void AddBook_WhenTitleHasLessThanThreeCharacters_ShouldThrowLibraryManagementException()
    {
        var service = CreateService();

        var exception = Assert.Throws<LibraryManagementException>(() => service.AddBook("Ab", "Author 1"));

        Assert.That(exception.Message, Does.Contain("Title"));
    }

    [Test]
    public void AddBook_WhenBookAlreadyExists_ShouldThrowDuplicateBookException()
    {
        var existingBooks = new[] { new Book(1, "Existing title", true, "Existing author") };
        var service = CreateService(books: existingBooks);

        Assert.Throws<DuplicateBookException>(() => service.AddBook("Existing title", "Existing author"));
    }

    private static LibraryService CreateService(
        IReadOnlyCollection<Book>? books = null,
        IReadOnlyCollection<Customer>? customers = null)
    {
        return new LibraryService(
            new FakeBookRepository(books ?? Array.Empty<Book>()),
            new FakeCustomerRepository(customers ?? Array.Empty<Customer>()));
    }

    private sealed class FakeBookRepository(IReadOnlyCollection<Book> books) : IBookRepository
    {
        private readonly List<Book> _books = books.ToList();

        public IReadOnlyCollection<Book> GetAll() => _books.AsReadOnly();

        public void AddBook(Book book) => _books.Add(book);
    }

    private sealed class FakeCustomerRepository(IReadOnlyCollection<Customer> customers) : ICustomerRepository
    {
        public IReadOnlyCollection<Customer> GetAll() => customers;
    }
}
