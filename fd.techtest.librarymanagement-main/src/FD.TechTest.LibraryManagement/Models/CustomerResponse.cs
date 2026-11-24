namespace FD.TechTest.LibraryManagement.Models;

/// <summary>
/// Représente un client retourné par l'API.
/// </summary>
public sealed record CustomerResponse(
    int Id,
    string Name,
    IReadOnlyCollection<BorrowedBookResponse> BorrowedBooks);

/// <summary>
/// Représente un emprunt retourné par l'API.
/// </summary>
public sealed record BorrowedBookResponse(int BookId, DateTime BorrowedAt);
