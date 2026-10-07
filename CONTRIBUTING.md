# Contributing to NearMe

## Workflow

1. Branch from `main`: `feat/<area>-<short-description>`, `fix/...`, `test/...`, `docs/...`, `chore/...`.
2. Keep changes small and focused on one concern.
3. Add or update tests with the change.
4. Run the checks below locally.
5. Open a pull request. CI must pass before merging. `main` must always build and pass its tests.

## Commit messages

Use [Conventional Commits](https://www.conventionalcommits.org/):

```
feat(auth): implement provider registration
fix(authz): prevent cross-provider listing updates
test(reviews): add review ownership tests
docs(security): document BOLA mitigation
chore(ci): cache NuGet packages
```

## Local checks

```bash
# Backend
cd backend && dotnet build && dotnet test

# Frontend
cd frontend && npm run lint && npm run format:check && npm test && npm run build
```

## Engineering rules

- **Architecture.** Respect the layer rules in [ADR-0002](docs/adr/0002-backend-project-dependency-rules.md).
  Controllers stay thin and call Application use cases; they never touch `NearMeDbContext` directly.
- **API.** Versioned business endpoints live under `/api/v1`. Errors use Problem Details. Collections
  are paginated.
- **Security.** Follow [SECURITY.md](SECURITY.md). New endpoints need authorization tests, including a
  cross-user (BOLA) test where a resource has an owner.
- **Dependencies.** Add a package only when a feature needs it. Backend versions live in
  `backend/Directory.Packages.props`.
- **Decisions.** Significant architecture, schema, API-contract or security decisions get an ADR in
  `docs/adr/`.
- **Database.** Schema changes go through EF Core migrations. Never edit an applied migration.
