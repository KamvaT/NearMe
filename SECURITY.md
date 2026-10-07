# Security Policy

NearMe handles personal data and location information, so security is treated as a design requirement
rather than a final step.

## Supported versions

NearMe is pre-release. Only the `main` branch receives security fixes.

## Reporting a vulnerability

Please **do not** open a public issue for security problems.

Report privately through GitHub's
[private vulnerability reporting](https://docs.github.com/code-security/security-advisories/guidance-on-reporting-and-writing-information-about-vulnerabilities/privately-reporting-a-security-vulnerability)
("Security" tab → "Report a vulnerability"). Include steps to reproduce, the affected endpoint or
component, and the impact you observed.

You can expect an acknowledgement within 7 days.

## Security principles in this codebase

These are the rules contributors must follow. Each is or will be backed by automated tests.

- **The server is the only security boundary.** Frontend checks are for usability only.
- **Every resource access is authorized** by authentication, role, ownership and resource state.
  Changing an ID in a request must never grant access to another user's data (BOLA/IDOR).
- **No mass assignment.** Requests bind to explicit request DTOs; roles, ownership, approval and
  moderation fields are never accepted from clients.
- **No excessive data exposure.** Responses use purpose-built DTOs; EF Core entities are never returned.
- **Location privacy.** Precise provider coordinates are never exposed publicly through any endpoint.
- **Uploads are untrusted.** Size, type and content are validated; stored names are generated server-side.
- **Errors do not leak internals.** Clients receive RFC 9457 Problem Details with a trace ID; stack
  traces, SQL errors and connection details go to server logs only.
- **No secrets in source control.** Local secrets use `dotnet user-secrets`; deployed secrets use the
  hosting platform's secret store.

## Current status

Implemented controls are listed in the README's *Implementation status* table. Controls described
above that relate to features not yet built (authentication, uploads, location) are requirements for
those features, not claims about the current codebase.
