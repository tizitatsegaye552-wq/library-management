using LibraryManagement.Data;
using LibraryManagement.Events;
using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ApplicationDbContext _context;
        private readonly EventBus _eventBus;

        public LibraryService(ApplicationDbContext context, EventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
        }

        public IReadOnlyList<Book> GetBooks() => _context.Books.ToList().AsReadOnly();

        public IReadOnlyList<Borrower> GetBorrowers() => _context.Borrowers.ToList().AsReadOnly();

        public IReadOnlyList<Transaction> GetAllTransactions() => _context.Transactions.ToList().AsReadOnly();

        public IReadOnlyList<Transaction> GetActiveTransactions() =>
            _context.Transactions.Where(t => t.ReturnedAt == null).ToList().AsReadOnly();

        public IReadOnlyList<Transaction> GetOverdueTransactions() =>
            _context.Transactions.ToList().Where(t => t.IsOverdue).ToList().AsReadOnly();

        public async Task<bool> CheckOutBookAsync(int bookId, int borrowerId)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            var borrower = _context.Borrowers.FirstOrDefault(b => b.Id == borrowerId);

            if (book == null || borrower == null || !book.IsAvailable)
                return false;

            book.IsAvailable = false;

            var transaction = new Transaction
            {
                BookId = book.Id,
                BookTitle = book.Title,
                BorrowerId = borrower.Id,
                BorrowerName = borrower.Name,
                BorrowerEmail = borrower.Email,
                CheckedOutAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _eventBus.PublishAsync(new BookCheckedOutEvent(
                book.Id, book.Title, borrower.Id, borrower.Name, borrower.Email, transaction.DueDate));

            return true;
        }

        public async Task<bool> ReturnBookAsync(int transactionId)
        {
            var transaction = _context.Transactions.FirstOrDefault(t => t.Id == transactionId && t.ReturnedAt == null);
            if (transaction == null) return false;

            var book = _context.Books.FirstOrDefault(b => b.Id == transaction.BookId);
            if (book != null)
            {
                book.IsAvailable = true;
            }

            transaction.ReturnedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _eventBus.PublishAsync(new BookReturnedEvent(
                transaction.BookId, transaction.BookTitle, transaction.BorrowerId, transaction.BorrowerName, transaction.BorrowerEmail, transaction.ReturnedAt.Value, transaction.IsOverdue, transaction.DaysOverdue));

            return true;
        }

        public async Task SendOverdueNoticesAsync()
        {
            var overdues = GetOverdueTransactions();

            foreach (var transaction in overdues)
            {
                var fee = transaction.DaysOverdue * 0.50m;
                
                await _eventBus.PublishAsync(new OverdueNoticeEvent(
                    transaction.BookId, transaction.BookTitle, transaction.BorrowerId, transaction.BorrowerName, transaction.BorrowerEmail, transaction.DaysOverdue, fee));
            }
        }

        public async Task AddBookAsync(Book book)
        {
            book.IsAvailable = true;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task AddBorrowerAsync(Borrower borrower)
        {
            _context.Borrowers.Add(borrower);
            await _context.SaveChangesAsync();
        }
    }
}
