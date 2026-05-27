namespace LibraryManagement.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int BorrowerId { get; set; }
        public string BorrowerName { get; set; } = string.Empty;
        public string BorrowerEmail { get; set; } = string.Empty;
        public DateTime CheckedOutAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal LateFee { get; set; }

        public bool IsReturned => ReturnedAt.HasValue;
        public bool IsOverdue => !IsReturned && DateTime.UtcNow > DueDate;
        public int DaysOverdue => IsOverdue
            ? (int)(DateTime.UtcNow - DueDate).TotalDays
            : 0;
    }
}
