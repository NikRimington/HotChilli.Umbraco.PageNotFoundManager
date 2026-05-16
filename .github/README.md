# Page Not Found Manager

![Logo](https://raw.githubusercontent.com/NikRimington/HotChilli.Umbraco.PageNotFoundManager/develop/docs/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/HotChilli.Umbraco.PageNotFound.svg)](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-blue.svg)](https://marketplace.umbraco.com/package/hotchilli.umbraco.pagenotfound)

## Supported Umbraco Versions

- **Version 2.x**: Supports Umbraco 9–13
- **Version 3.x (beta)**: Supports Umbraco 14 *(see note below)*
- **Version 17.x**: Supports Umbraco 17

> **Note:** There is also a `dev/umbv15` branch that targets Umbraco 15. This version was never officially released but should work if needed.

## Installation

Install the package from [NuGet](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/) or the [Umbraco Marketplace](https://marketplace.umbraco.com/package/hotchilli.umbraco.pagenotfound):

```powershell
# For Umbraco 9–13
PM> Install-Package HotChilli.Umbraco.PageNotFound -Version 2.*

# For Umbraco 14 (beta)
PM> Install-Package HotChilli.Umbraco.PageNotFound -Version 3.*-beta

# For Umbraco 17
PM> Install-Package HotChilli.Umbraco.PageNotFound -Version 17.*
```

## Usage

Once installed, the package registers automatically via an Umbraco Composer. For advanced scenarios, use the `UsePageNotFoundManager` extension method on `IUmbracoBuilder` to control when it loads.

Once running, a "Manage 404 Page" action appears on content nodes in the backoffice. This opens a dialog to assign a 404 page for that node and its children, allowing different 404 pages to be configured for different sections of the website.

### Relation Tracking

When a 404 page is assigned, the package creates a native Umbraco relation (`pageNotFoundManagerRelated`) between the content node and its configured 404 page. This means the 404 page will appear as a tracked reference in the backoffice, preventing accidental deletion without warning.

### Health Check

A health check is included under the "Page Not Found Manager" group in the Umbraco health check dashboard. It verifies that all configured 404 page references point to content nodes that still exist. If any orphaned references are found (e.g. the 404 page was deleted), the health check reports them and provides an action to remove them automatically.

## Development Branches

- `dev/v2`: Umbraco 9–13 (stable)
- `dev/v3`: Umbraco 14 (beta)
- `dev/umbv15`: Umbraco 15 (unreleased, but should work)
- `dev/umbv17`: Umbraco 17 (active development)

## Special Thanks

This package is a port of [PageNotFoundManager](https://github.com/TimGeyssens/UmbracoPageNotFoundManager) by [Tim Geyssens](https://twitter.com/timgeyssens), created with his permission. The code base used as a reference was taken prior to the licensing model change.

## Credits

The logo uses [404](https://thenounproject.com/term/404/3283006) from the [Noun Project](https://thenounproject.com) by [Ilham Fitrotul Hayat](https://thenounproject.com/fhilham), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).
