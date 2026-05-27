using LibraryManagement.Events;
using LibraryManagement.Services;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Records the accrued late fee onto the transaction record
    /// and updates the overdue-notices counter in StatisticsService.
    /// Fee rule: $0.50 per day overdue.
    /// </summary>
    public class OverdueNoticeFeeHandler : IEventHandler<OverdueNoticeEvent>
    {
        private readonly StatisticsService _stats;
        private readonly ILogger<OverdueNoticeFeeHandler> _logger;

        public OverdueNoticeFeeHandler(
            StatisticsService stats,
            ILogger<OverdueNoticeFeeHandler> logger)
        {
            _stats = stats;
            _logger = logger;
        }

        public Task HandleAsync(OverdueNoticeEvent e)
        {
            _stats.IncrementOverdueNotices();
            _stats.AddFee(e.FeeAccrued);

            _logger.LogWarning(
                "💰 [Fee] Late fee of ${Fee:F2} recorded for Borrower #{BorrowerId}. " +
                "Total fees collected: ${TotalFees:F2}",
                e.FeeAccrued, e.BorrowerId, _stats.TotalFeesCollected);
            return Task.CompletedTask;
        }
    }
}
