# 💰 Finance Tracker

A personal finance tracking web app built with Blazor WebAssembly and ASP.NET Core Web API.

> 🚧 This project is currently under active development.

---

## Tech Stack

- **Frontend** — Blazor WebAssembly (.NET 8)
- **Backend** — ASP.NET Core Web API (.NET 8)
- **Database** — SQLite + Entity Framework Core
- **Auth** — JWT Bearer + Refresh Tokens
- **UI Components** — Radzen Blazor

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
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
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

---

## Status

- [x] Project setup & solution structure
- [x] EF Core models & migrations
- [x] Authentication (register, login, refresh tokens)
- [ ] Transactions CRUD
- [ ] Categories CRUD
- [ ] Budgets
- [ ] Dashboard

---

## Author

**Your Name** — [GitHub](https://github.com/Melvinots)
