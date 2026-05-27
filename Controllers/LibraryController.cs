using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Services;
using LibraryManagement.Models;

using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly StatisticsService _stats;

        public LibraryController(ILibraryService libraryService, StatisticsService stats)
        {
            _libraryService = libraryService;
            _stats          = stats;
        }

        // GET /Library — Dashboard
        public IActionResult Index()
        {
            ViewBag.Books           = _libraryService.GetBooks();
            ViewBag.Borrowers       = _libraryService.GetBorrowers();
            ViewBag.ActiveCheckouts = _libraryService.GetActiveTransactions();
            ViewBag.Stats           = _stats;
            return View();
        }

        // GET /Library/Books
        public IActionResult Books()
        {
            var books = _libraryService.GetBooks();
            return View(books);
        }

        // GET /Library/Borrowers
        public IActionResult Borrowers()
        {
            var borrowers = _libraryService.GetBorrowers();
            return View(borrowers);
        }

        // GET /Library/Transactions
        public IActionResult Transactions()
        {
            var transactions = _libraryService.GetAllTransactions();
            return View(transactions);
        }

        // GET /Library/Overdue
        public IActionResult Overdue()
        {
            ViewBag.OverdueTransactions = _libraryService.GetOverdueTransactions();
            ViewBag.Stats               = _stats;
            return View();
        }

        // POST /Library/CheckOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int bookId, int borrowerId)
        {
            try
            {
                await _libraryService.CheckOutBookAsync(bookId, borrowerId);
                TempData["Success"] = "✅ Book checked out successfully! Check the console for handler logs.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"❌ {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Library/Return
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int transactionId)
        {
            try
            {
                await _libraryService.ReturnBookAsync(transactionId);
                TempData["Success"] = "✅ Book returned successfully! Check the console for handler logs.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"❌ {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Library/SendOverdueNotices
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOverdueNotices()
        {
            await _libraryService.SendOverdueNoticesAsync();
            TempData["Success"] = "✅ Overdue notices sent! Check the console for handler logs.";
            return RedirectToAction(nameof(Overdue));
        }

        // POST /Library/AddBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook([Bind("Title,Author,ISBN,Genre")] Book book)
        {
            if (ModelState.IsValid)
            {
                await _libraryService.AddBookAsync(book);
                TempData["Success"] = $"✅ Book '{book.Title}' registered successfully.";
                return RedirectToAction(nameof(Books));
            }
            TempData["Error"] = "❌ Failed to register book. Please check the details.";
            return RedirectToAction(nameof(Books));
        }

        // POST /Library/AddBorrower
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBorrower([Bind("Name,Email,Phone")] Borrower borrower)
        {
            if (ModelState.IsValid)
            {
                await _libraryService.AddBorrowerAsync(borrower);
                TempData["Success"] = $"✅ Borrower '{borrower.Name}' registered successfully.";
                return RedirectToAction(nameof(Borrowers));
            }
            TempData["Error"] = "❌ Failed to register borrower. Please check the details.";
            return RedirectToAction(nameof(Borrowers));
        }
    }
}
