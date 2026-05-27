namespace LibraryManagement.Events
{
    /// <summary>
    /// Raised when a book has not been returned past its due date.
    /// Fired once per overdue transaction when a librarian triggers the notice run.
    /// </summary>
    public record OverdueNoticeEvent(
        int BookId,
        string BookTitle,
        int BorrowerId,
        string BorrowerName,
        string BorrowerEmail,
        int DaysOverdue,
        decimal FeeAccrued
    ) : IEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
