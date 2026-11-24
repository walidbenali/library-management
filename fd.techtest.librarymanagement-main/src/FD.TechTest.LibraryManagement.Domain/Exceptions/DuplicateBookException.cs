namespace FD.TechTest.LibraryManagement.Domain.Exceptions
{
    public class DuplicateBookException : LibraryManagementException
    {
        public DuplicateBookException(string message) : base(message)
        {
        }
    }
}
