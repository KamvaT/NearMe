# ADR-0003: Unversioned liveness and readiness endpoints

- **Status:** Accepted
- **Date:** 2026-10-07

## Context

Business endpoints are versioned under `/api/v1`. The Repository Structure Specification (§64, §71)
defines `GET /health` and optionally `/health/ready`. Health probes are consumed by infrastructure
(Azure App Service health checks, uptime monitors, the frontend's connectivity indicator), not by
API clients, and must not change when the business API version changes.

## Decision

- `GET /health`: **liveness**. Reports that the process is running and checks no dependencies, so a
  database outage does not cause the platform to restart a healthy process.
- `GET /health/ready`: **readiness**. Runs checks tagged `ready` (currently database connectivity) and
  returns `503` when any fails.
- Both are outside `/api/v1`, allow anonymous access, and return **only** `{"status":"healthy|degraded|unhealthy"}`.
  Check names, exception messages and connection details are never returned (Security Architecture:
  no sensitive diagnostic information publicly).

## Consequences

- Probe URLs are stable across API versions.
- Diagnosing which dependency failed requires server logs, which is intended.
- Both endpoints will be rate-limited with the rest of the API when rate limiting is introduced.
