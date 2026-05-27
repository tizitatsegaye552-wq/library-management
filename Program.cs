using LibraryManagement.Services;
using LibraryManagement.EventHandlers;
using LibraryManagement.Events;
using LibraryManagement.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ── Authentication ───────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.AccessDeniedPath = "/Account/Login";
    });

// ── MVC ──────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Infrastructure ────────────────────────────────────────────────────
// EventBus: singleton — stateless router, safe for concurrent requests
builder.Services.AddSingleton<EventBus>();

// AuthService: scoped — now depends on DbContext
builder.Services.AddScoped<AuthService>();

// StatisticsService: singleton — holds shared in-memory counters
builder.Services.AddSingleton<StatisticsService>();

// ── Business Services ─────────────────────────────────────────────────
// LibraryService: scoped — per-request, owns in-memory data
builder.Services.AddScoped<ILibraryService, LibraryService>();

// ── Event Handlers — BookCheckedOutEvent (3 handlers) ─────────────────
builder.Services.AddTransient<IEventHandler<BookCheckedOutEvent>, BookCheckedOutEmailHandler>();
builder.Services.AddTransient<IEventHandler<BookCheckedOutEvent>, BookCheckedOutLoggingHandler>();
builder.Services.AddTransient<IEventHandler<BookCheckedOutEvent>, BookCheckedOutStatisticsHandler>();

// ── Event Handlers — BookReturnedEvent (3 handlers) ───────────────────
builder.Services.AddTransient<IEventHandler<BookReturnedEvent>, BookReturnedEmailHandler>();
builder.Services.AddTransient<IEventHandler<BookReturnedEvent>, BookReturnedLoggingHandler>();
builder.Services.AddTransient<IEventHandler<BookReturnedEvent>, BookReturnedInventoryHandler>();

// ── Event Handlers — OverdueNoticeEvent (3 handlers) ──────────────────
builder.Services.AddTransient<IEventHandler<OverdueNoticeEvent>, OverdueNoticeEmailHandler>();
builder.Services.AddTransient<IEventHandler<OverdueNoticeEvent>, OverdueNoticeLoggingHandler>();
builder.Services.AddTransient<IEventHandler<OverdueNoticeEvent>, OverdueNoticeFeeHandler>();

// ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Database Initialization & Seeding ─────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    if (!db.Users.Any())
    {
        var hasher = new PasswordHasher<AppUser>();
        
        var admin = new AppUser { Email = "admin@library.com", Role = "Admin", FullName = "System Admin" };
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");
        
        var librarian = new AppUser { Email = "librarian@library.com", Role = "Librarian", FullName = "Demo Librarian" };
        librarian.PasswordHash = hasher.HashPassword(librarian, "password123");

        db.Users.AddRange(admin, librarian);
        
        // Seed Books
        db.Books.AddRange(
            new Book { Title = "Clean Code", Author = "Robert C. Martin", Genre = "Technology", ISBN = "978-0132350884" },
            new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Genre = "Technology", ISBN = "978-0135957059" },
            new Book { Title = "Design Patterns", Author = "Erich Gamma", Genre = "Technology", ISBN = "978-0201633610" },
            new Book { Title = "Dune", Author = "Frank Herbert", Genre = "Science Fiction", ISBN = "978-0441172719" },
            new Book { Title = "1984", Author = "George Orwell", Genre = "Dystopian", ISBN = "978-0451524935" }
        );

        // Seed Borrowers
        db.Borrowers.AddRange(
            new Borrower { Name = "Alice Johnson", Email = "alice@library.com", Phone = "555-0101" },
            new Borrower { Name = "Bob Smith", Email = "bob@library.com", Phone = "555-0102" },
            new Borrower { Name = "Charlie Davis", Email = "charlie@library.com", Phone = "555-0103" }
        );

        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Default route → Home Index (Landing)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
