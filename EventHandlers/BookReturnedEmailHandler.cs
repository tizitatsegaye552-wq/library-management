using LibraryManagement.Events;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Simulates sending a return confirmation email.
    /// Includes a "late return" notice if the book was overdue.
    /// </summary>
    public class BookReturnedEmailHandler : IEventHandler<BookReturnedEvent>
    {
        private readonly ILogger<BookReturnedEmailHandler> _logger;

        public BookReturnedEmailHandler(ILogger<BookReturnedEmailHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BookReturnedEvent e)
        {
            await Task.Delay(50);
            var lateNote = e.WasLate
                ? $" (⚠️ {e.LateDays} day(s) late)"
                : " (✅ on time)";

            _logger.LogInformation(
                "📧 [Email] Return confirmation sent to {BorrowerName} ({Email}) — " +
                "Book: '{BookTitle}'{LateNote}",
                e.BorrowerName, e.BorrowerEmail, e.BookTitle, lateNote);
        }
    }
}
