using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Transaction -> Book / Borrower relationships
            modelBuilder.Entity<Transaction>()
                .HasOne<Book>()
                .WithMany()
                .HasForeignKey(t => t.BookId);

            modelBuilder.Entity<Transaction>()
                .HasOne<Borrower>()
                .WithMany()
                .HasForeignKey(t => t.BorrowerId);
        }
    }
}
