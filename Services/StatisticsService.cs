using System.Threading;

namespace LibraryManagement.Services
{
    /// <summary>
    /// Singleton service that holds in-memory library statistics.
    /// Uses Interlocked for thread-safe counter increments.
    /// Counters reset when the app restarts (in-memory, by design).
    /// </summary>
    public class StatisticsService
    {
        private int _totalCheckouts;
        private int _totalReturns;
        private int _totalOverdueNotices;
        private decimal _totalFeesCollected;
        private readonly object _feeLock = new();

        public int TotalCheckouts => _totalCheckouts;
        public int TotalReturns => _totalReturns;
        public int TotalOverdueNotices => _totalOverdueNotices;
        public decimal TotalFeesCollected
        {
            get { lock (_feeLock) return _totalFeesCollected; }
        }

        public void IncrementCheckouts() =>
            Interlocked.Increment(ref _totalCheckouts);

        public void IncrementReturns() =>
            Interlocked.Increment(ref _totalReturns);

        public void IncrementOverdueNotices() =>
            Interlocked.Increment(ref _totalOverdueNotices);

        public void AddFee(decimal amount)
        {
            lock (_feeLock)
                _totalFeesCollected += amount;
        }
    }
}
