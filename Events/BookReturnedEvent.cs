namespace LibraryManagement.Events
{
    /// <summary>
    /// Raised when a borrower returns a book to the library.
    /// </summary>
    public record BookReturnedEvent(
        int BookId,
        string BookTitle,
        int BorrowerId,
        string BorrowerName,
        string BorrowerEmail,
        DateTime ReturnedAt,
        bool WasLate,
        int LateDays
    ) : IEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
