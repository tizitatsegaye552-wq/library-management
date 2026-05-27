namespace LibraryManagement.Events
{
    /// <summary>
    /// Raised when a borrower checks out a book from the library.
    /// </summary>
    public record BookCheckedOutEvent(
        int BookId,
        string BookTitle,
        int BorrowerId,
        string BorrowerName,
        string BorrowerEmail,
        DateTime DueDate
    ) : IEvent
    {
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
