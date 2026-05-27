using LibraryManagement.Events;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Simulates sending an overdue notice email to the borrower.
    /// </summary>
    public class OverdueNoticeEmailHandler : IEventHandler<OverdueNoticeEvent>
    {
        private readonly ILogger<OverdueNoticeEmailHandler> _logger;

        public OverdueNoticeEmailHandler(ILogger<OverdueNoticeEmailHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(OverdueNoticeEvent e)
        {
            await Task.Delay(50);
            _logger.LogWarning(
                "📧 [Email] OVERDUE NOTICE sent to {BorrowerName} ({Email}) — " +
                "Book: '{BookTitle}' | {DaysOverdue} days overdue | Fee: ${FeeAccrued:F2}",
                e.BorrowerName, e.BorrowerEmail, e.BookTitle,
                e.DaysOverdue, e.FeeAccrued);
        }
    }
}
