# Backend — HC.PageNotFoundManager

C# / .NET 10 / Razor SDK package project.

## Key Directories

| Path | Purpose |
|------|---------|
| `Startup/` | `StartupComposer` (Umbraco `IComposer`) wires DI, content finder, cache refresher, Swagger. `UmbracoStartingNotificationHandler` runs DB migrations on startup. `UmbracoBuilderExtensions.UsePageNotFoundManager()` is the public API. |
| `Config/` | `IPageNotFoundService` / `PageNotFoundConfigService` — manages 404 mappings with `IAppPolicyCache` and ancestor-chain inheritance. Settings from `HCS:PageNotFoundManager` config section. |
| `ContentFinders/` | `PageNotFoundFinder` implements `IContentLastChanceFinder` — intercepts failed routes, serves the configured 404 page with multi-domain support. |
| `Backoffice/` | `ManagementController` — REST API at `/api/v1/hcs`, endpoints: `get-not-found`, `set-not-found`. Includes Swagger/OpenAPI config. |
| `Migrations/` | Creates `PageNotFoundManagerConfig` table (`ParentId` GUID PK, `NotFoundPageId` GUID). Supports legacy migration from earlier versions. |
| `Caching/` | `PageNotFoundCacheRefresher` — distributed cache invalidation for server farm support. |

## Umbraco Extension Points Used

`IContentLastChanceFinder`, `IComposer`, `INotificationAsyncHandler`, `PayloadCacheRefresherBase`, `AsyncMigrationBase`

## Database Schema

Table: `PageNotFoundManagerConfig`
- `ParentId` GUID — PK, the content node the rule applies to
- `NotFoundPageId` GUID — the 404 page to serve
