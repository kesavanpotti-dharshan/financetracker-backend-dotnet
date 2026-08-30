# FinanceTracker — Backend

.NET Web API backend for FinanceTracker, a personal finance tracker: accounts, credit cards, AI-powered PDF statement extraction, and transactions.

Frontend repo: [financetracker-frontend-react](https://github.com/kesavanpotti-dharshan/financetracker-frontend-react)

## Tech stack

- .NET 10 Web API
- PostgreSQL (Neon), via EF Core + Npgsql
- JWT access tokens + refresh token rotation (httpOnly cookie)
- Azure Blob Storage for uploaded PDF statements
- Google Gemini (gemini-3.6-flash) for statement data extraction
- Built-in `Microsoft.AspNetCore.OpenApi` (no Swashbuckle)

## Architecture

Clean Architecture, four projects:
src/
├── FinanceTracker.Domain/ Entities and enums. No dependencies on anything else.
├── FinanceTracker.Application/ Use cases (commands/handlers), DTOs, and interfaces
│ that Infrastructure implements. Depends only on Domain.
├── FinanceTracker.Infrastructure/ EF Core (AppDbContext, migrations, repositories),
│ JWT/password hashing, Gemini client, Azure Blob storage.
│ Implements the interfaces Application defines.
└── FinanceTracker.Api/ Controllers, Program.cs, DI wiring, auth middleware.

The only project that knows about all the others.

Dependencies point inward — Domain knows nothing about Application, Infrastructure, or Api. Application defines interfaces (`IAccountRepository`, `IStatementParser`, `IFileStorage`, etc.) that Infrastructure implements, so swapping a piece (e.g. Azure Blob for another storage provider, or the Gemini REST call for the official SDK) only touches Infrastructure.

### Data model

`Users` → `Accounts` (checking/savings/investment/credit card) → `AccountBalances` (a history table, not a single column — so balance trends and "current balance" are both cheap) → `CreditCardDetails` (1:1 with a credit-card account) → `Statements` (uploaded PDFs, with the raw AI extraction kept as `jsonb` for auditability) → `Transactions` (either auto-inserted from a confirmed statement, or added manually).

### Auth flow

Register/login issue a short-lived JWT access token (returned in the response body) and a refresh token (stored server-side hashed, delivered to the client as an httpOnly cookie). `/api/auth/refresh` rotates the refresh token on every use — the old one is revoked and chained to its replacement, so a stolen refresh token can only be used once before the rotation invalidates it. All account-scoped endpoints filter by the authenticated user's ID at the repository level, so cross-user data access isn't possible even with a valid token for a different account.

### Statement extraction pipeline

1. `POST /api/statements/upload/{accountId}` — PDF is stored in Azure Blob (private container), then sent inline to Gemini with a strict JSON-schema prompt.
2. Gemini's response (balance, due date, transactions, etc.) is stored as-is in `Statements.RawExtractedJson` — nothing is trusted or committed yet.
3. The frontend shows the extracted data for review.
4. `POST /api/statements/{id}/confirm` — the user-confirmed balance is written to `AccountBalances`, and the extracted transactions are parsed and inserted into `Transactions`. Malformed individual transactions are skipped rather than failing the whole confirm.

## Getting started

```bash
git clone https://github.com/kesavanpotti-dharshan/financetracker-backend-dotnet.git
cd financetracker-backend-dotnet

dotnet restore

# secrets (see table below)
dotnet user-secrets init --project src/FinanceTracker.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<neon-pooled-connection-string>" --project src/FinanceTracker.Api
dotnet user-secrets set "ConnectionStrings:MigrationConnection" "<neon-direct-connection-string>" --project src/FinanceTracker.Api
dotnet user-secrets set "Jwt:SigningKey" "<random-256-bit-key>" --project src/FinanceTracker.Api
dotnet user-secrets set "Azure:BlobConnectionString" "<azure-storage-connection-string>" --project src/FinanceTracker.Api
dotnet user-secrets set "Azure:BlobContainerName" "statements" --project src/FinanceTracker.Api
dotnet user-secrets set "Gemini:ApiKey" "<gemini-api-key>" --project src/FinanceTracker.Api

# migrate (use the direct/non-pooler connection for this)
dotnet ef database update --project src/FinanceTracker.Infrastructure --startup-project src/FinanceTracker.Api --connection "<neon-direct-connection-string>"

dotnet run --project src/FinanceTracker.Api --launch-profile https
```

Local HTTPS requires trusting the .NET dev cert once: `dotnet dev-certs https --trust`.

## Configuration

| Secret                                  | Description                                                                      |
| --------------------------------------- | -------------------------------------------------------------------------------- |
| `ConnectionStrings:DefaultConnection`   | Neon pooled connection string, used at runtime                                   |
| `ConnectionStrings:MigrationConnection` | Neon direct (non-pooler) connection string, used for `dotnet ef database update` |
| `Jwt:SigningKey`                        | Symmetric key for signing access tokens                                          |
| `Azure:BlobConnectionString`            | Azure Storage account connection string                                          |
| `Azure:BlobContainerName`               | Blob container name for uploaded statements (private access)                     |
| `Gemini:ApiKey`                         | Google Gemini API key, sent via `x-goog-api-key` header                          |

Non-secret config (`Jwt:Issuer`, `Jwt:Audience`, `Jwt:AccessTokenMinutes`, `Jwt:RefreshTokenDays`) lives in `appsettings.json`.

## API surface

- `POST /api/auth/{register,login,refresh,logout}`
- `GET/POST/PUT/DELETE /api/accounts` (+ `/balance`, `/credit-card` sub-routes)
- `GET/POST /api/institutions`
- `POST /api/statements/upload/{accountId}`, `GET /api/statements/{id}`, `POST /api/statements/{id}/confirm`
- `GET/POST/PUT/DELETE /api/accounts/{accountId}/transactions`

OpenAPI document available at `/openapi/v1.json` in development.

## Status

Personal-use project, actively developed. Not yet deployed.
