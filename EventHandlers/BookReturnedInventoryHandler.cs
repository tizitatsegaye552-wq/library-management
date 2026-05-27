using LibraryManagement.Events;
using LibraryManagement.Services;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Marks the returned book as available in the in-memory inventory
    /// and increments the returns counter in StatisticsService.
    /// </summary>
    public class BookReturnedInventoryHandler : IEventHandler<BookReturnedEvent>
    {
        private readonly StatisticsService _stats;
        private readonly ILogger<BookReturnedInventoryHandler> _logger;

        public BookReturnedInventoryHandler(
            StatisticsService stats,
            ILogger<BookReturnedInventoryHandler> logger)
        {
            _stats = stats;
            _logger = logger;
        }

        public Task HandleAsync(BookReturnedEvent e)
        {
            // The actual Book.IsAvailable flag is updated directly in LibraryService.
            // This handler tracks the statistics counter.
            _stats.IncrementReturns();
            _logger.LogInformation(
                "📦 [Inventory] Book #{BookId} '{BookTitle}' is now available. " +
                "Total returns: {Total}",
                e.BookId, e.BookTitle, _stats.TotalReturns);
            return Task.CompletedTask;
        }
    }
}
