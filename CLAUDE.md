# PageNotFound Manager

Umbraco v13 (net8.0) package. Allows admins to configure per-domain 404 pages. Published to NuGet as `HotChilli.Umbraco.PageNotFound`.

## Structure

```
src/
  HC.PageNotFoundManager/       # Main C# project (NuGet package)
    wwwroot/                    # Static FE assets (Vite output — do not commit)
    Backoffice/                 # Umbraco backoffice controllers
    Caching/, Config/, Constants/, ContentFinders/, Migrations/, Models/, Startup/
  hc.pagenotfoundmanager.client/ # Vite/TypeScript frontend source
  PageNotFoundManager.sln
```

## Build

**Frontend** (must run before dotnet build):
```
cd src/hc.pagenotfoundmanager.client
npm ci
npm run build
```

**Backend:**
```
dotnet build src\HC.PageNotFoundManager\HC.PageNotFoundManager.csproj --configuration Release
dotnet pack src\PageNotFoundManager.sln --no-build --output nupkgs
```

## CI/CD

- `build.yml` — triggers on push/PR to `develop`, builds pre-release package (`0.1.0-pre{run}`)
- `build-and-release.yml` — triggers on version tags (`v1.2.3`), publishes to NuGet via `NUGET_API_KEY` secret

Both workflows: build FE first, then .NET 8 only.

## Notes

- `wwwroot/*.js` and `*.js.map` are gitignored — built by CI and locally via `npm run build`
- Static backoffice assets in `wwwroot/Backoffice/` and `wwwroot/js/` are committed
- Test site at `src/TestSites/v16/` for local Umbraco instance
