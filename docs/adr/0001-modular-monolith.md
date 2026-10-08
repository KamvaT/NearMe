# ADR-0001: Modular monolith architecture

- **Status:** Accepted
- **Date:** 2026-10-07

## Context

NearMe is an MVP built and operated by a single developer. Its domains (identity, providers, listings,
search, reviews, administration) share one relational data model and are tightly connected: search
ranks providers using listings, location, ratings and availability in a single query.

## Decision

Build NearMe as a **modular monolith**: one ASP.NET Core API process deployed as one unit, internally
split into layered projects (Api, Application, Domain, Infrastructure, Shared) with enforced
dependency rules (ADR-0002), backed by one SQL Server database. The React SPA is deployed separately.

## Consequences

- One deployment, one database, and in-process calls: simple to run, test, debug and host cheaply.
- Transactions across features (e.g. moderation plus audit log) are ordinary database transactions.
- Boundaries depend on discipline, so they are enforced by architecture tests.
- If a module ever needs independent scaling, the layer boundaries make extraction possible, but this
  is not expected for the MVP.

## Alternatives considered

- **Microservices:** rejected. Distributed transactions, service discovery, multiple deployments and
  network failure modes add cost with no benefit at this scale or team size.
- **Single-project API:** rejected. It provides no structural protection against business logic
  leaking into controllers or data access leaking into the domain.
