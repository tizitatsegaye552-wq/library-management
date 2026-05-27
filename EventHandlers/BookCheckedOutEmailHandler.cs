using LibraryManagement.Events;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Simulates sending a checkout confirmation email to the borrower.
    /// In production, replace Task.Delay with an actual SMTP/SendGrid call.
    /// </summary>
    public class BookCheckedOutEmailHandler : IEventHandler<BookCheckedOutEvent>
    {
        private readonly ILogger<BookCheckedOutEmailHandler> _logger;

        public BookCheckedOutEmailHandler(ILogger<BookCheckedOutEmailHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BookCheckedOutEvent e)
        {
            await Task.Delay(50); // Simulate async email send
            _logger.LogInformation(
                "📧 [Email] Checkout confirmation sent to {BorrowerName} ({Email}) — " +
                "Book: '{BookTitle}' | Due: {DueDate:dd MMM yyyy}",
                e.BorrowerName, e.BorrowerEmail, e.BookTitle, e.DueDate);
        }
    }
}
