# ADR-0004: Defer the first EF Core migration to the first real schema

- **Status:** Accepted
- **Date:** 2026-10-07

## Context

The Phase 1 Definition of Done (Repository Structure Specification §72) includes "initial migration
works". In Phase 1 `NearMeDbContext` intentionally has no entities. The Project Instructions place the
database model in Phase 3 and authentication (ASP.NET Core Identity tables) in Phase 4.

The Phase 1 scaffold was produced in an environment where NuGet packages and `dotnet ef` could not run,
so a migration could only have been written by hand, which is error-prone and unverifiable.

## Decision

Phase 1 verifies the EF Core → SQL Server path with:

- `NearMeDbContext` registered against SQL Server;
- a `/health/ready` database connectivity check;
- integration tests against a real SQL Server in CI (`EnsureCreated` on a disposable database).

The first migration (`InitialCreate`) is generated with `dotnet ef migrations add` when the first
entities are introduced, and the integration tests then switch from `EnsureCreated` to `Migrate`
plus a `HasPendingModelChanges` check.

## Consequences

- The "initial migration works" Phase 1 criterion moves to the start of the database phase. This is a
  documented deviation from §72, not an omission.
- No empty or hand-written migration enters the history.
