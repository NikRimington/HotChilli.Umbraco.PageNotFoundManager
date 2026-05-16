# Frontend — hc.pagenotfoundmanager.client

TypeScript / Lit 3 / Vite 6. Builds to `../HC.PageNotFoundManager/wwwroot/` (gitignored — never commit build output).

## Key Files

| Path | Purpose |
|------|---------|
| `src/index.ts` | Entry point. Registers entity action + modal manifests, sets up Umbraco auth context for the OpenAPI client. |
| `src/Actions/Entity/pagenotfound.entityaction.ts` | Extends `UmbEntityActionBase`. Appears on document context menus. Alias: `hcs.pagenotfound.entity.action`. |
| `src/Modals/pagenotfound.modal.element.ts` | Extends `UmbModalBaseElement`. Sidebar modal with content picker for selecting 404 pages. |
| `src/Modals/pagenotfound.modal.token.ts` | Modal token and type definitions. |
| `src/api/` | **Auto-generated** from OpenAPI spec via `npm run generate-client`. Never edit `client.gen.ts`, `sdk.gen.ts`, or `types.gen.ts` by hand. |

## Vite Config Notes

- All `@umbraco-cms/*` packages are externalized — not bundled.
- Entry point (`hcs.pagenotfoundmanager.js`) has a stable name; chunks use content hashes.
- Output goes directly to `../HC.PageNotFoundManager/wwwroot/` to be picked up as static web assets at `/App_Plugins/HCS.PageNotFoundManager`.

## Regenerating the API Client

Requires a running Umbraco instance. Run `npm run generate-client` to pull the OpenAPI spec and regenerate `src/api/`.
