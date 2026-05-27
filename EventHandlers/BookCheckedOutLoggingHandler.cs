using LibraryManagement.Events;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Writes a structured audit log entry every time a book is checked out.
    /// </summary>
    public class BookCheckedOutLoggingHandler : IEventHandler<BookCheckedOutEvent>
    {
        private readonly ILogger<BookCheckedOutLoggingHandler> _logger;

        public BookCheckedOutLoggingHandler(ILogger<BookCheckedOutLoggingHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BookCheckedOutEvent e)
        {
            await Task.Run(() =>
                _logger.LogInformation(
                    "📝 [Audit] CHECKOUT | Book #{BookId} '{BookTitle}' | " +
                    "Borrower #{BorrowerId} '{BorrowerName}' | At: {OccurredAt:u}",
                    e.BookId, e.BookTitle, e.BorrowerId, e.BorrowerName, e.OccurredAt));
        }
    }
}
