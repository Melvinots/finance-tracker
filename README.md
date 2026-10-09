# 💰 Finance Tracker

A personal finance tracking web app built with Blazor WebAssembly and ASP.NET Core Web API.

Track income and expenses, set monthly budgets, and see where your money goes on a dashboard.

---

## Features

**Accounts**
- Register and sign in with email and password
- JWT bearer authentication; the API issues an access token and a refresh token on login

**Transactions**
- Add, edit, and delete income and expense transactions
- Search by description, filter by type (all, income, expense), and filter by one or more categories
- Filters stay applied after you delete a transaction

**Categories and budgets**
- Create, edit, and delete categories
- Set monthly budgets and monitor progress against them

**Dashboard**
- Monthly summary: total income, total expenses, net savings, and transaction count
- Spending overview chart over the last several months
- Spending-by-category breakdown for the selected month
- Period selector to review past months

**Settings and UI**
- Configurable currency in user settings
- Landing page and sign-in / register screens styled to match the Radzen theme
- Responsive layout down to mobile widths

---

## Not Implemented Yet

These are known gaps, not bugs.

- [ ] **Forgot password.** `/forgot-password` shows a placeholder notice. A real flow needs a reset-token endpoint, an email service, and a "set new password" page.
- [ ] **Dark mode.** Only the light theme is available.
- [ ] **Comprehensive data export.** Export is limited; a full export (all transactions, categories, and budgets in common formats) is planned.
- [ ] **Using refresh tokens.** Refresh tokens are issued but never used. Every login creates a new access token and refresh token pair, even if the current token is still valid, and nothing refreshes an expiring session silently.
- [ ] **Automatic budgets for future months.** Budgets are created manually; they don't carry over to upcoming months on their own.

---

## Tech Stack

- **Frontend** — Blazor WebAssembly (.NET 10)
- **Backend** — ASP.NET Core Web API (.NET 10)
- **Database** — SQLite + Entity Framework Core
- **Auth** — JWT Bearer
- **UI Components** — Radzen Blazor, Blazicons (Lucide)

---

## Project Structure

```
FinanceTracker.sln
├── FinanceTracker/           # ASP.NET Core Web API
├── FinanceTracker.Client/    # Blazor WebAssembly
└── FinanceTracker.Shared/    # Shared DTOs and models
```

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- dotnet-ef CLI: `dotnet tool install --global dotnet-ef`

### Run the Server
```bash
cd FinanceTracker
dotnet ef database update
dotnet run
```

### Run the Client
```bash
cd FinanceTracker.Client
dotnet run
```

The client calls the API at `https://localhost:7022`. Start the server first.

---

## Roadmap

Ideas for a future version, in no particular order:

- Password reset by email
- Dark mode
- Full data export and import
- Proper session handling: reuse valid tokens, silently refresh them, and persist them across reloads
- Automatic or recurring budgets for future months
- Recurring transactions
- Caching of rarely changing data such as categories and settings

---

## Author

**Melvin** — [GitHub](https://github.com/Melvinots)
