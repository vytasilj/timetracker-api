# Time Tracker API

A time tracking and invoicing API built for freelance/contract work. Built to replace spreadsheet-based hour tracking with a structured client → project → time entry data model, plus automatic monthly earnings reports.

Frontend: [timetracker-app](https://github.com/vytasilj/timetracker-app)

## Features

- Client / Project / Time Entry management (CRUD)
- Flexible time logging: enter hours directly, or start/end time with automatic 30-minute lunch break deduction
- Per-entry hourly rate overrides (falls back to the project's default rate)
- Monthly summary report, grouped by client and project
- JWT authentication with PBKDF2 password hashing
- Fully containerized with Docker, deployed to Railway

## Tech stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core + PostgreSQL
- JWT Bearer authentication
- xUnit (unit tests for time calculation logic)
- Docker (multi-stage build)
- Scalar for interactive API documentation (development only)

## Getting started

**Prerequisites:** .NET 10 SDK, Docker

```bash
git clone https://github.com/vytasilj/timetracker-api.git
cd timetracker-api/TimeTracker.Api

# Start local PostgreSQL
cd ..
docker compose up -d
cd TimeTracker.Api

# Configure local secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=timetracker;Username=timetracker;Password=localdev123"
dotnet user-secrets set "Jwt:SecretKey" "<generate a random 64-byte base64 string>"
dotnet user-secrets set "Auth:SeedUserEmail" "your@email.com"
dotnet user-secrets set "Auth:SeedUserPassword" "your-password"

# Apply database schema
dotnet ef database update

# Run
dotnet run
```

On first startup, a user account is automatically created from the `Auth:SeedUserEmail` / `Auth:SeedUserPassword` secrets — there's no public registration endpoint, since this API is designed for a single user.

API documentation is available at `/scalar/v1` when running in the Development environment.

## Running tests

```bash
dotnet test
```

## Deployment

Deployed to [Railway](https://railway.app) via Docker, with a managed PostgreSQL instance. Migrations are applied manually against the production database using `dotnet ef database update --connection "..."`.