# 💰 Finance Tracker

A personal finance tracking web app built with Blazor WebAssembly and ASP.NET Core Web API.

Track income and expenses, set monthly budgets, and see where your money goes on a dashboard.

---

## Features

**Accounts**
- Register and sign in with email and password
- JWT bearer authentication; the API issues an access token and a refresh token on login
- Export your data as CSV
- Deactivate your account (signing in again reactivates it)

**Transactions**
- Add, edit, and delete income and expense transactions
- Search by description, filter by type (all, income, expense), and filter by one or more categories
- Filters stay applied after you delete a transaction

**Categories and budgets**
- Create, edit, and delete categories
- Deleting a category moves its transactions to a default category
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
- [ ] **Comprehensive data export.** Export is CSV only; a full export (all transactions, categories, and budgets in common formats) is planned.
- [ ] **Using refresh tokens.** Refresh tokens are issued but never used. Every login creates a new access token and refresh token pair, even if the current token is still valid, and nothing refreshes an expiring session silently.
- [ ] **Automatic budgets for future months.** Budgets are created manually; they don't carry over to upcoming months on their own.

---

## Tech Stack

- **Frontend** — Blazor WebAssembly (.NET 10)
- **Backend** — ASP.NET Core Web API (.NET 10)
- **Database** — SQLite + Entity Framework Core
- **Auth** — JWT Bearer
- **UI Components** — Radzen Blazor, Blazicons (Lucide)
- **Testing** — xUnit, Moq

---

## Project Structure

```
FinanceTracker.sln
├── FinanceTracker/           # ASP.NET Core Web API
├── FinanceTracker.Client/    # Blazor WebAssembly
├── FinanceTracker.Shared/    # Shared DTOs and models
└── FinanceTracker.Tests/     # Unit tests
```

---

## API

The API follows REST conventions: resource-based URLs, HTTP verbs for the action, and meaningful status codes. All endpoints except `auth` require a bearer token.

| Resource | Endpoints |
| -------- | --------- |
| Auth | `POST /api/auth/register`<br>`POST /api/auth/login`<br>`POST /api/auth/refresh`<br>`POST /api/auth/revoke`<br>`POST /api/auth/forgot-password` |
| Transactions | `GET /api/transactions`<br>`GET /api/transactions/{id}`<br>`POST /api/transactions`<br>`PUT /api/transactions/{id}`<br>`DELETE /api/transactions/{id}` |
| Categories | `GET /api/categories`<br>`GET /api/categories/{id}`<br>`GET /api/categories/{id}/transaction-count`<br>`POST /api/categories`<br>`PUT /api/categories/{id}`<br>`DELETE /api/categories/{id}` |
| Budgets | `GET /api/budgets?month=&year=`<br>`GET /api/budgets/{id}`<br>`POST /api/budgets`<br>`PUT /api/budgets/{id}`<br>`DELETE /api/budgets/{id}` |
| Dashboard | `GET /api/dashboard?month=&year=` |
| User settings | `GET /api/user-settings`<br>`PUT /api/user-settings` |
| Account | `GET /api/account/export`<br>`DELETE /api/account` (deactivate) |

If `month` and `year` are omitted, they default to the current month and year.

### Error handling

Every error is returned as `application/problem+json` with the same shape, including model-validation failures and rejected tokens:

```json
{
  "status": 409,
  "title": "Conflict",
  "detail": "Email already registered.",
  "instance": "/api/auth/register",
  "traceId": "00-abc123...",
  "errors": { "Email": ["Email already registered."] }
}
```

`errors` is only present when a specific field is at fault. `traceId` matches the server log entry for that request.

Services throw typed exceptions, and `ExceptionHandlingMiddleware` maps them to status codes:

| Exception | Status | Used for |
| --------- | ------ | -------- |
| `ValidationFailedException` | 400 | Invalid or missing input, including body fields that reference missing data |
| `AuthFailedException` | 401 | Wrong credentials, invalid or expired refresh token |
| `NotFoundException` | 404 | The resource in the URL doesn't exist |
| `ConflictException` | 409 | Duplicate email, deleting a protected category |
| Anything else | 500 | Generic message plus `traceId`; the real exception is only logged |

Only these typed exceptions expose their message to the client. Framework and unexpected exceptions never do.

### Client error handling

The Blazor client turns failed responses into an `ApiException` with a `Kind` (`Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Network`, `Timeout`, `Server`), field errors, and a `TraceId`. Its `Message` is always safe to show to users, and technical details (method, URL, status) are kept separately for the browser console. Pages branch on `Kind`, not on message text, and `AppNotifier` shows the toast and logs the details.

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- dotnet-ef CLI: `dotnet tool install --global dotnet-ef`

### Run the Application
1. Open `FinanceTracker.sln` in Visual Studio.
2. In **Solution Explorer**, right-click the `FinanceTracker` project.
3. Select **Set as Startup Project**.
4. Open **Tools → NuGet Package Manager → Package Manager Console**, or use a terminal in the project folder.
5. Run the database migrations:
   ```bash
   cd FinanceTracker
   dotnet ef database update
   ```
6. Press **F5** or click **Start** to run the application.

The `FinanceTracker` project hosts both the API and the Blazor WebAssembly client, so you don't need to run `FinanceTracker.Client` separately.

### Run the Tests

```bash
dotnet test
```

### Test Account

Use these credentials to sign in:

| Field    | Value              |
| -------- | ------------------ |
| Email    | `test@example.com` |
| Password | `default123`       |

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
- Integration tests that call each API route end to end

---

## Author

**Melvin** — [GitHub](https://github.com/Melvinots)
