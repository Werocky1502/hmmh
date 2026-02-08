---
description: "HMMH monorepo conventions and phase guidance"
applyTo: "**/*"
---

# HMMH Monorepo Instructions

These rules apply to all changes in this repository unless a more specific instruction overrides them.

## Repository Goals
- Build a modular, extensible monorepo for the HelpMeManageHealth (HMMH) web application.
- Maintain clear boundaries between UI, API, and infrastructure projects.
- Favor maintainability and predictable structure over rapid shortcuts.

## Monorepo Structure
- Use Nx as the monorepo tool.
- Keep apps under an apps/ directory and shared libraries under libs/.
- Ensure commands are executed from the repository root.
- Use npm workspaces for dependency management and deduplication.

## UI (React + TypeScript + Mantine)
- Use React with TypeScript and Mantine as the primary UI library.
- Prefer composing Mantine components over custom CSS when possible.
- If custom CSS is needed, keep it in a co-located .css file next to the component.
- No UI tests in the first iteration.

## API (.NET + EF Core)
- Use the latest .NET SDK (currently .NET 10) and EF Core.
- Build a REST API following standard ASP.NET Core patterns.
- No API tests in the first iteration.

## Database (PostgreSQL)
- Provide a Docker-based PostgreSQL setup scoped to the API project.
- Use docker-compose for local development workflows.

## Implementation Phases
1. Add custom instruction file(s) and update README. Commit.
2. Initialize Nx monorepo with base config. Commit.
3. Add UI app with demo page and lint/format setup. Commit.
4. Add API app with demo endpoint and Dockerfile. Commit.
5. Add PostgreSQL container setup under API. Commit.

## Commits
- Make a commit after each phase listed above.
- Keep commit messages short and action-oriented.
