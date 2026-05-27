using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public interface ILibraryService
    {
        IReadOnlyList<Book> GetBooks();
        IReadOnlyList<Borrower> GetBorrowers();
        IReadOnlyList<Transaction> GetAllTransactions();
        IReadOnlyList<Transaction> GetActiveTransactions();
        IReadOnlyList<Transaction> GetOverdueTransactions();

        Task<bool> CheckOutBookAsync(int bookId, int borrowerId);
        Task<bool> ReturnBookAsync(int transactionId);
        Task SendOverdueNoticesAsync();

        Task AddBookAsync(Book book);
        Task AddBorrowerAsync(Borrower borrower);
    }
}
