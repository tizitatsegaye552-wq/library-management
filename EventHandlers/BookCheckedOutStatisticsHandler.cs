using LibraryManagement.Events;
using LibraryManagement.Services;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Increments the total checkout counter held by the singleton StatisticsService.
    /// </summary>
    public class BookCheckedOutStatisticsHandler : IEventHandler<BookCheckedOutEvent>
    {
        private readonly StatisticsService _stats;
        private readonly ILogger<BookCheckedOutStatisticsHandler> _logger;

        public BookCheckedOutStatisticsHandler(
            StatisticsService stats,
            ILogger<BookCheckedOutStatisticsHandler> logger)
        {
            _stats = stats;
            _logger = logger;
        }

        public Task HandleAsync(BookCheckedOutEvent e)
        {
            _stats.IncrementCheckouts();
            _logger.LogInformation(
                "📊 [Stats] Checkout recorded. Total checkouts so far: {Total}",
                _stats.TotalCheckouts);
            return Task.CompletedTask;
        }
    }
}
