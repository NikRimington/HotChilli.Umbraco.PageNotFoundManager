# CLAUDE.md

Guidance for Claude Code working in this repository.

## Project Overview

HotChilli.Umbraco.PageNotFoundManager — Umbraco CMS package letting administrators configure custom 404 pages per content node. Adds a "404 Manager" context menu action on document nodes. Child nodes inherit 404 config from ancestors unless explicitly overridden.

**Version branches:** `dev/v2` (Umbraco 9-13), `dev/v3` (Umbraco 14), `dev/umbv15`, `dev/umbv17` (current). Main dev branch: `develop`.

## Build

### Backend (.NET)
```bash
dotnet build src/PageNotFoundManager.sln --configuration Release
dotnet pack src/HC.PageNotFoundManager/HC.PageNotFoundManager.csproj --configuration Release
```

### Frontend (Vite/Lit) — Node.js 22 required
```bash
cd src/hc.pagenotfoundmanager.client
npm install
npm run build      # outputs to src/HC.PageNotFoundManager/wwwroot/
npm run watch      # dev mode
npm run generate-client  # regenerate API client from running Umbraco OpenAPI spec
```

Frontend build artifacts in `wwwroot/` are gitignored — CI builds them before `dotnet pack`.

## Code Conventions

- **C#:** PascalCase types/members, `I` prefix on interfaces, block-scoped namespaces, nullable enabled. Full rules in `src/.editorconfig`.
- **TypeScript:** Strict mode, ES2020 target, ESNext modules, experimental decorators enabled.
- **Constants:** `Constants/Constants.cs` — base alias `HCS`, backoffice alias `HCS.PageNotFound`.

## CI/CD

`.github/workflows/`:
- **build.yml** — push to `develop` + PRs. Builds frontend, then builds and packs .NET on `windows-latest`.
- **build-and-release.yml** — version tags (`v*.*.*`). Builds frontend, packs, pushes to NuGet.
