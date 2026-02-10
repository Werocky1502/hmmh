# HMMH
HelpMeManageHealth (HMMH) is a modular web application to track user weight and daily calorie intake.
It is built as an Nx monorepo with a React + TypeScript UI, a .NET REST API, and a PostgreSQL database.

## Repository Structure (planned)
- apps/hmmh-ui: React + TypeScript + Mantine UI
- apps/hmmh-api: ASP.NET Core API with EF Core
- libs/: Shared code and utilities
- apps/hmmh-api/infra/db: PostgreSQL container setup for local development

## Implementation Phases
1. Add repo-level instructions and update documentation.
2. Initialize Nx monorepo with base configuration.
3. Add UI app with demo page and lint/format setup.
4. Add API app with demo endpoint and Dockerfile.
5. Add PostgreSQL container setup for API development.
6. Add authentication and user management (Identity + JWT + UI pages).
7. Add weight management (weight entries API + dashboard and weights page UI).
8. Add calorie management (calorie entries API + dashboard and calories page UI).
9. Refactor auth to in-API OIDC (OpenIddict) with token refresh.
10. Refactor API into layered architecture (services, repositories, factories, DI extensions).

## Configuration Notes
- UI reads the API base URL from `VITE_API_BASE_URL` (defaults to same origin).
- API token lifetimes live in `appsettings.json` under `OpenIddict`.
- OAuth token endpoint is available at `/connect/token`.
