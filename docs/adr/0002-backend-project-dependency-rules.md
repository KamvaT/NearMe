# ADR-0002: Backend project dependency rules

- **Status:** Accepted
- **Date:** 2026-10-07

## Context

The Project Instructions list `Api → Application`, while the Implementation & Repository Structure
Specification (§71) also lists `Api → Infrastructure`. Both are needed: something must register
Infrastructure implementations (EF Core, later Identity and Blob Storage) in dependency injection,
and that composition root is the API host.

## Decision

Allowed project references:

| Project                 | May reference                       |
| ----------------------- | ----------------------------------- |
| `NearMe.Shared`         | nothing                             |
| `NearMe.Domain`         | Shared                              |
| `NearMe.Application`    | Domain, Shared                      |
| `NearMe.Infrastructure` | Application, Domain, Shared         |
| `NearMe.Api`            | Application, Infrastructure, Shared |

`NearMe.Api → NearMe.Infrastructure` exists **only for composition** (Program.cs, DI registration,
endpoint wiring such as health checks). Controllers must depend on Application abstractions and
never on Infrastructure types such as `NearMeDbContext`.

`NearMe.Domain` has no package or framework dependencies.

## Enforcement

`backend/tests/NearMe.UnitTests/Architecture` fails the build if:

- a `.csproj` adds a project reference outside the table above;
- a new source project is added without a rule;
- `NearMe.Domain` gains a package or framework reference;
- the compiled Domain/Application/Shared assemblies reference forbidden assemblies (EF Core,
  ASP.NET Core, Azure SDKs, outer layers);
- a controller's constructor or fields use an Infrastructure type.

## Consequences

- Layering violations are caught in CI rather than in code review.
- The Api project can technically see Infrastructure types; the controller rule covers the risk that
  matters (data access from controllers). Other misuse is caught in review.
