# ADR-0005: Runtime and toolchain versions

- **Status:** Accepted
- **Date:** 2026-10-07

## Context

The specifications name technologies but not versions. Choices made at scaffolding time decide the
support window of the whole project.

## Decision

| Component | Version | Reason |
| --- | --- | --- |
| .NET / ASP.NET Core / EF Core | 10 (LTS) | .NET 8 reaches end of support in November 2026; .NET 10 is supported until November 2028. Pinned by `backend/global.json` (roll forward within 10.0). |
| NuGet versions | Central Package Management | One place (`backend/Directory.Packages.props`) to review and update versions; NuGet audit reports vulnerable transitive packages. |
| Node.js | 22 or later (CI uses 22) | Supported LTS line that satisfies Vite 8's engine requirement. |
| React / Vite / Tailwind CSS / React Router | 19 / 8 / 4 / 8 | Current stable releases at scaffolding time. |
| ESLint | 9 (pinned below 10) | `eslint-plugin-jsx-a11y` does not yet support ESLint 10. Accessibility linting is a project requirement, so ESLint stays on 9 until the plugin supports 10. |
| Linting/formatting | ESLint + Prettier | Named in the Frontend Architecture Specification; the Vite template's default (oxlint) was replaced. |

## Consequences

- Revisit the ESLint pin when `eslint-plugin-jsx-a11y` releases ESLint 10 support.
- Dependency updates are reviewed through pull requests; automated update tooling (e.g. Dependabot)
  is planned for the security-hardening phase.
