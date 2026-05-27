# Library Management System — Architecture Design

## 1. System Overview

This system demonstrates **Event-Driven Architecture (EDA)** built on **ASP.NET Core 8 MVC**. Instead of services calling each other directly, they communicate through typed *events* dispatched via a central `EventBus`. This decouples producers from consumers and makes the system easy to extend.

---

## 2. Core Concepts

| Term | Definition |
|------|-----------|
| **Event** | An immutable record describing something that *happened* (past tense) |
| **EventBus** | In-process dispatcher that routes events to all registered handlers |
| **Handler** | A class that reacts to a specific event type (email, log, stats…) |
| **Publisher** | A service that raises an event after completing business logic |

---

## 3. Architecture Layers

```
┌─────────────────────────────────────────────────────┐
│                   Presentation Layer                │
│         LibraryController  ←→  Razor Views          │
└─────────────────────┬───────────────────────────────┘
                      │ calls ILibraryService
┌─────────────────────▼───────────────────────────────┐
│                  Application Layer                  │
│              LibraryService (Scoped)                │
│     CheckOutBookAsync / ReturnBookAsync /           │
│     SendOverdueNoticesAsync                         │
└─────────────────────┬───────────────────────────────┘
                      │ publishes via EventBus
┌─────────────────────▼───────────────────────────────┐
│               Event Infrastructure                  │
│            EventBus (Singleton)                     │
│  PublishAsync<T>() → GetServices<IEventHandler<T>>  │
└──────┬──────────────┬──────────────┬────────────────┘
       │              │              │
 BookCheckedOut  BookReturned  OverdueNotice
 ──────────────  ────────────  ─────────────
 EmailHandler    EmailHandler  EmailHandler
 LoggingHandler  LoggingHandler LoggingHandler
 StatsHandler    InventoryHandler FeeHandler
```

---

## 4. Event Catalogue

### 4.1 `BookCheckedOutEvent`

| Property | Type | Description |
|----------|------|-------------|
| `BookId` | `int` | Unique book identifier |
| `BookTitle` | `string` | Human-readable title |
| `BorrowerId` | `int` | Unique borrower identifier |
| `BorrowerName` | `string` | Full name |
| `DueDate` | `DateTime` | Return deadline (14 days out) |
| `OccurredAt` | `DateTime` | UTC timestamp of checkout |

**Handlers triggered:**
1. `BookCheckedOutEmailHandler` — simulates confirmation email
2. `BookCheckedOutLoggingHandler` — writes structured audit entry
3. `BookCheckedOutStatisticsHandler` — increments `TotalCheckouts`

---

### 4.2 `BookReturnedEvent`

| Property | Type | Description |
|----------|------|-------------|
| `BookId` | `int` | Book being returned |
| `BookTitle` | `string` | Human-readable title |
| `BorrowerId` | `int` | Who returned it |
| `ReturnedAt` | `DateTime` | Actual return time |
| `WasLate` | `bool` | Whether returned after due date |
| `LateDays` | `int` | Days past due (0 if on time) |
| `OccurredAt` | `DateTime` | UTC timestamp |

**Handlers triggered:**
1. `BookReturnedEmailHandler` — simulates return-confirmation email
2. `BookReturnedLoggingHandler` — writes audit entry
3. `BookReturnedInventoryHandler` — marks book as available

---

### 4.3 `OverdueNoticeEvent`

| Property | Type | Description |
|----------|------|-------------|
| `BookId` | `int` | Overdue book |
| `BookTitle` | `string` | Human-readable title |
| `BorrowerId` | `int` | Responsible borrower |
| `BorrowerName` | `string` | Full name |
| `BorrowerEmail` | `string` | For email dispatch |
| `DaysOverdue` | `int` | How many days past due |
| `FeeAccrued` | `decimal` | Calculated late fee |
| `OccurredAt` | `DateTime` | UTC timestamp |

**Handlers triggered:**
1. `OverdueNoticeEmailHandler` — simulates overdue notice email
2. `OverdueNoticeLoggingHandler` — writes audit entry
3. `OverdueNoticeFeeHandler` — records fee in transaction record

---

## 5. Handler Responsibility Matrix

| Handler | BookCheckedOut | BookReturned | OverdueNotice |
|---------|:-:|:-:|:-:|
| Email Notification | ✅ | ✅ | ✅ |
| Audit Logging | ✅ | ✅ | ✅ |
| Statistics (counts) | ✅ | — | — |
| Inventory Update | — | ✅ | — |
| Fee Calculation | — | — | ✅ |

---

## 6. Dependency Injection Strategy

```csharp
// Lifetimes chosen for correctness and performance
Singleton  → EventBus          (stateless router)
Singleton  → StatisticsService (shared mutable counters)
Scoped     → ILibraryService   (per-request data access)
Transient  → IEventHandler<T>  (stateless; cheap to create)
```

> **Why Singleton EventBus?**  
> It holds no state of its own — it delegates immediately to `IServiceProvider`. Safe for concurrent requests.

> **Why Transient handlers?**  
> Handlers are stateless. Creating a new instance per publish call avoids any shared-state issues across concurrent events.

---

## 7. Data Flow Example — Book Checkout

```
1. User clicks "Check Out" in browser (POST /Library/CheckOut)
       │
2. LibraryController.CheckOutBook(bookId, borrowerId)
       │
3. LibraryService.CheckOutBookAsync()
       │  creates BookCheckedOutEvent record
       │
4. EventBus.PublishAsync<BookCheckedOutEvent>(event)
       │  resolves IServiceProvider.GetServices<IEventHandler<BookCheckedOutEvent>>()
       │
       ├──► BookCheckedOutEmailHandler.HandleAsync(event)
       │        logs: "📧 Email sent to Alice: 'Clean Code' due 2026-06-07"
       │
       ├──► BookCheckedOutLoggingHandler.HandleAsync(event)
       │        logs: "📝 Checkout #1 | Book #1 | Borrower #1 | 2026-05-24T..."
       │
       └──► BookCheckedOutStatisticsHandler.HandleAsync(event)
                logs: "📊 Total checkouts now: 1"
       │
5. Controller redirects to /Library (dashboard refreshes)
```

---

## 8. Project Folder Structure

```
LibraryManagement/
├── Events/                         ← Event record types
│   ├── IEvent.cs
│   ├── BookCheckedOutEvent.cs
│   ├── BookReturnedEvent.cs
│   └── OverdueNoticeEvent.cs
│
├── EventHandlers/                  ← Handler implementations
│   ├── IEventHandler.cs
│   ├── BookCheckedOutEmailHandler.cs
│   ├── BookCheckedOutLoggingHandler.cs
│   ├── BookCheckedOutStatisticsHandler.cs
│   ├── BookReturnedEmailHandler.cs
│   ├── BookReturnedLoggingHandler.cs
│   ├── BookReturnedInventoryHandler.cs
│   ├── OverdueNoticeEmailHandler.cs
│   ├── OverdueNoticeLoggingHandler.cs
│   └── OverdueNoticeFeeHandler.cs
│
├── Services/                       ← Business logic + EventBus
│   ├── EventBus.cs
│   ├── StatisticsService.cs
│   ├── ILibraryService.cs
│   └── LibraryService.cs
│
├── Models/                         ← Domain models
│   ├── Book.cs
│   ├── Borrower.cs
│   └── Transaction.cs
│
├── Controllers/
│   └── LibraryController.cs
│
├── Views/
│   └── Library/
│       ├── Index.cshtml            ← Dashboard
│       ├── Return.cshtml
│       └── Overdue.cshtml
│
├── Program.cs                      ← DI registration
└── README.md
```

---

## 9. Extension Points

The EDA pattern makes these extensions trivial — zero changes to existing code:

| Future Feature | How to Add |
|----------------|-----------|
| SMS notifications | Add `BookCheckedOutSmsHandler` + register as `IEventHandler<BookCheckedOutEvent>` |
| Push notifications | Same pattern — new handler, new registration |
| Async/queue-based delivery | Replace `EventBus.PublishAsync` internals; handlers unchanged |
| Persistent event log (Event Sourcing) | Add `EventStoreHandler` that writes every event to DB |
| Unit testing handlers | Inject mock `ILogger`; no EventBus needed in handler tests |

---

## 10. Technology Stack

| Component | Technology |
|-----------|-----------|
| Runtime | .NET 8 |
| Framework | ASP.NET Core MVC |
| Language | C# 12 |
| DI Container | Microsoft.Extensions.DependencyInjection (built-in) |
| Logging | Microsoft.Extensions.Logging (ILogger<T>) |
| Data Store | In-memory `List<>` (no DB dependency) |
| View Engine | Razor (.cshtml) |
| Version Control | Git + GitLab |
