using LibraryManagement.Events;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Writes a structured audit log entry for every overdue notice dispatched.
    /// </summary>
    public class OverdueNoticeLoggingHandler : IEventHandler<OverdueNoticeEvent>
    {
        private readonly ILogger<OverdueNoticeLoggingHandler> _logger;

        public OverdueNoticeLoggingHandler(ILogger<OverdueNoticeLoggingHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(OverdueNoticeEvent e)
        {
            await Task.Run(() =>
                _logger.LogWarning(
                    "📝 [Audit] OVERDUE | Book #{BookId} '{BookTitle}' | " +
                    "Borrower #{BorrowerId} '{BorrowerName}' | " +
                    "DaysOverdue: {DaysOverdue} | FeeAccrued: ${FeeAccrued:F2} | At: {OccurredAt:u}",
                    e.BookId, e.BookTitle, e.BorrowerId, e.BorrowerName,
                    e.DaysOverdue, e.FeeAccrued, e.OccurredAt));
        }
    }
}
