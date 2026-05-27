# 📚 Library Management System — Event-Driven Architecture

> **ASP.NET Core 8 MVC** project demonstrating **Event-Driven Architecture (EDA)**
> using in-process events, a typed EventBus, and ASP.NET Core's built-in DI container.

---

## 👥 Team Members

| # | Name | Student ID |
|---|------|-----------|
| 1 | *(your name)* | *(your ID)* |
| 2 | *(teammate)* | *(ID)* |
| 1 | *(your name)* | *(your ID)* |
| 2 | *(teammate)* | *(ID)* ||
| 2 | *(teammate)* | *(ID)* |
| 2 | *(teammate)* | *(ID)* |
| 2 | *(teammate)* | *(ID)* |

---

## ⚡ Events Implemented

| Event | Handlers |
|-------|---------|
| `BookCheckedOutEvent` | EmailHandler · LoggingHandler · StatisticsHandler |
| `BookReturnedEvent`   | EmailHandler · LoggingHandler · InventoryHandler  |
| `OverdueNoticeEvent`  | EmailHandler · LoggingHandler · FeeHandler        |

**Total: 3 events × 3 handlers = 9 handler classes**

---

## 🏗️ Architecture

```
HTTP Request
     │
     ▼
LibraryController
     │
     ▼
LibraryService  ──publishes──►  EventBus  ──resolves via DI──► Handlers[]
```

### Key Design Rules
- Events are **C# `record` types** — immutable by default
- `EventBus` is **Singleton** — stateless, safe for concurrent requests
- `LibraryService` is **Scoped** — per-request lifecycle
- All handlers are **Transient** — stateless, cheapest to allocate
- EventBus continues to next handler even if one throws (fault isolation)

---

## 🚀 Setup & Run

```bash
# Clone repository
git clone <repo-url>
cd LibraryManagement

# Restore NuGet packages
& "C:\Program Files\dotnet\dotnet.exe" restore

# Build
& "C:\Program Files\dotnet\dotnet.exe" build

# Run (development)
& "C:\Program Files\dotnet\dotnet.exe" run

# Visit the app
# http://localhost:5000  or  https://localhost:5001
```

---

## 🧪 Testing the EDA Flow

1. **Open the terminal** where `dotnet run` is running — handler logs appear here
2. **Go to Dashboard** → `/Library`
3. **Check out a book** — watch 3 log lines appear:
   ```
   📧 [Email]  Checkout confirmation sent to Alice...
   📝 [Audit]  CHECKOUT | Book #1 'Clean Code' | Borrower #1...
   📊 [Stats]  Checkout recorded. Total checkouts so far: 1
   ```
4. **Return the book** — 3 different handler logs appear:
   ```
   📧 [Email]  Return confirmation sent to Alice...
   📝 [Audit]  RETURN | Book #1...
   📦 [Inventory]  Book #1 is now available. Total returns: 1
   ```
5. **Go to Overdue** → `/Library/Overdue` → click "Send All Overdue Notices"
   ```
   📧 [Email]   OVERDUE NOTICE sent to Bob...
   📝 [Audit]   OVERDUE | Book #2...
   💰 [Fee]     Late fee of $7.00 recorded...
   ```

---

## 📁 Project Structure

```
LibraryManagement/
├── Events/                    ← 3 record types (IEvent)
├── EventHandlers/             ← 9 handler classes (IEventHandler<T>)
├── Services/
│   ├── EventBus.cs            ← In-process event dispatcher
│   ├── StatisticsService.cs   ← Singleton counters
│   └── LibraryService.cs      ← Business logic + event publishing
├── Models/                    ← Book, Borrower, Transaction
├── Controllers/
│   └── LibraryController.cs
├── Views/Library/
│   ├── Index.cshtml           ← Dashboard
│   └── Overdue.cshtml
├── wwwroot/css/library.css    ← Custom dark UI
├── Program.cs                 ← DI registrations (all 9 handlers)
└── ARCHITECTURE.md            ← Detailed design document
```

---

## 📐 Adding a New Handler (Extension Point)

To add a 4th handler for `BookCheckedOutEvent` (e.g., SMS):

1. Create `EventHandlers/BookCheckedOutSmsHandler.cs` implementing `IEventHandler<BookCheckedOutEvent>`
2. Add one line to `Program.cs`:
   ```csharp
   builder.Services.AddTransient<IEventHandler<BookCheckedOutEvent>, BookCheckedOutSmsHandler>();
   ```
**Zero changes to `EventBus`, `LibraryService`, or any existing handler.** This is the power of EDA.

---

## 🛠️ Tech Stack

| Component | Technology |
|-----------|-----------|
| Runtime   | .NET 8    |
| Framework | ASP.NET Core MVC |
| Language  | C# 12 |
| DI        | Microsoft.Extensions.DependencyInjection |
| Logging   | Microsoft.Extensions.Logging |
| Data      | In-memory `List<>` |
| UI        | Razor Views + Custom CSS (glassmorphism) |
