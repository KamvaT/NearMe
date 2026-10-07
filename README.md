# NearMe

**Local services. Right where you are.**

NearMe is a campus and local-community marketplace that helps students discover, compare and contact
affordable small businesses, student entrepreneurs and independent service providers near them — a
barber two streets away, a tutor in the next residence, a phone repair down the road.

It is being built as a professional portfolio project with security treated as a first-class
requirement: role-based access control, resource-ownership checks (BOLA/IDOR prevention), location
privacy and secure file uploads.

> **Project status: Phase 1 — engineering foundation.**
> The repository currently contains the runnable skeleton (API, frontend, tests, CI). No marketplace
> features are implemented yet. See [Implementation status](#implementation-status).

## Architecture

A **modular monolith**: one deployable API with strict internal layering, and a separate SPA.

```
React (Vite, Tailwind)  ──HTTPS/REST──▶  ASP.NET Core Web API
                                           ├─ NearMe.Api             HTTP, routing, error handling
                                           ├─ NearMe.Application     use cases, DTOs, abstractions
                                           ├─ NearMe.Domain          entities and business rules
                                           ├─ NearMe.Infrastructure  EF Core, SQL Server, (Identity, Blob Storage)
                                           └─ NearMe.Shared          cross-cutting primitives
                                                    │
                                                    ▼
                                         SQL Server / Azure SQL
```

Dependency direction is enforced by automated tests, not convention — see
[ADR-0002](docs/adr/0002-backend-project-dependency-rules.md).

| Area     | Technology                                                                   |
| -------- | ---------------------------------------------------------------------------- |
| Frontend | React 19, JavaScript, Vite 8, Tailwind CSS 4, React Router                    |
| Backend  | C#, ASP.NET Core Web API (.NET 10 LTS), EF Core                               |
| Database | SQL Server (local), Azure SQL (production)                                    |
| Testing  | xUnit, WebApplicationFactory, Vitest, React Testing Library                   |
| CI       | GitHub Actions                                                               |
| Planned  | ASP.NET Core Identity, Azure Blob Storage, Leaflet/OpenStreetMap, Vercel + Azure |

## Repository layout

```
backend/          .NET solution (src/ and tests/)
frontend/         React application
docs/             requirements, architecture, API, database, UX, security, testing, deployment, ADRs
infrastructure/   deployment infrastructure (planned)
scripts/          developer scripts (planned)
tests/            cross-application tests such as end-to-end (planned)
.github/workflows CI pipeline
```

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/)
- SQL Server: LocalDB (Windows, installed with Visual Studio) or SQL Server in Docker

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/NearMe.Api --launch-profile http
```

The API listens on `http://localhost:5080`.

- `GET /health` — liveness, returns `{"status":"healthy"}`
- `GET /health/ready` — readiness, includes a database connectivity check (`503` if unreachable)
- `GET /openapi/v1.json` — OpenAPI document (Development environment only)

### Frontend

```bash
cd frontend
cp .env.example .env.local
npm ci
npm run dev
```

The app runs on `http://localhost:5173` and shows whether it can reach the API.

### Configuration

Secrets are never committed. Local development uses
[user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

```bash
cd backend/src/NearMe.Api
# Only needed if you are not using the LocalDB default in appsettings.Development.json:
dotnet user-secrets set "ConnectionStrings:NearMeDatabase" "Server=localhost,1433;Database=NearMe;User Id=sa;Password=<your-local-password>;TrustServerCertificate=True"
```

All configuration names are listed in [`.env.example`](.env.example).

## Testing

```bash
# Backend: unit + API tests (no external services needed)
cd backend
dotnet test

# Backend: SQL Server integration tests (skipped unless a disposable test database is provided)
NEARME_TEST_SQL_CONNECTION="Server=localhost,1433;Database=NearMe_IntegrationTests;User Id=sa;Password=<pw>;TrustServerCertificate=True" \
  dotnet test tests/NearMe.IntegrationTests

# Frontend
cd frontend
npm run lint
npm test
```

| Suite                     | What it covers today                                                                 |
| ------------------------- | ------------------------------------------------------------------------------------- |
| `NearMe.UnitTests`        | Architecture/dependency rules; exception handler does not leak internals              |
| `NearMe.ApiTests`         | Health probes, Problem Details errors, CORS allow-list, OpenAPI hidden outside dev    |
| `NearMe.IntegrationTests` | EF Core ↔ SQL Server connectivity; readiness probe against a real database            |
| Frontend (Vitest)         | API client error/timeout handling, routing, skip link, API status component          |

## Implementation status

| Capability                                             | Status         |
| ------------------------------------------------------ | -------------- |
| Solution structure, layering rules, CI                 | ✅ Implemented |
| Health/readiness endpoints, Problem Details, CORS      | ✅ Implemented |
| EF Core + SQL Server connection                        | ✅ Implemented |
| Database schema and migrations                         | ⏳ Next phases |
| Authentication and roles (ASP.NET Core Identity)       | ⏳ Planned     |
| Providers, listings, categories, search, location      | ⏳ Planned     |
| Reviews, favourites, enquiries, admin moderation       | ⏳ Planned     |
| Image uploads (Azure Blob Storage)                     | ⏳ Planned     |
| Deployment (Vercel + Azure)                            | ⏳ Planned     |

## Documentation

- [Architecture Decision Records](docs/adr/README.md)
- [Security policy](SECURITY.md)
- [Contributing](CONTRIBUTING.md)
