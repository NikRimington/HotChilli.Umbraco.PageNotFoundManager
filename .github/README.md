# Page Not Found Manager

![Logo](https://raw.githubusercontent.com/NikRimington/HotChilli.Umbraco.PageNotFoundManager/develop/docs/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/HotChilli.Umbraco.PageNotFound.svg)](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-blue.svg)](https://marketplace.umbraco.com/package/hotchilli.umbraco.pagenotfound)

## Getting Started

Page Not Found Manager supports Umbraco v13+.

Packages are available via NuGet, with a listing on the [Umbraco Marketplace](https://marketplace.umbraco.com/package/hotchilli.umbraco.pagenotfound) for discoverability.

## Installation

To [install from NuGet](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/), run the following command in your instance of Visual Studio.

    PM> Install-Package HotChilli.Umbraco.PageNotFound

## How to use

Once installed, the package registers automatically via an Umbraco Composer. If you need more control over when it loads, an extension method for `IUmbracoBuilder` called `UsePageNotFoundManager` is also available.

Once running, a "Manage 404 Page" action appears on content nodes in the backoffice. This opens a dialog to assign a 404 page for that node and its children, allowing different 404 pages to be configured for different parts of the website.

### Relation Tracking

When a 404 page is assigned, the package creates a native Umbraco relation (`pageNotFoundManagerRelated`) between the content node and its configured 404 page. This means the 404 page will appear as a tracked reference in the backoffice, preventing accidental deletion without warning.

### Health Check

A health check is included under the "Page Not Found Manager" group in the Umbraco health check dashboard. It verifies that all configured 404 page references point to content nodes that still exist. If any orphaned references are found (e.g. the 404 page was deleted), the health check reports them and provides an action to remove them automatically.

## Special Thanks

This package is a port of [PageNotFoundManager](https://github.com/TimGeyssens/UmbracoPageNotFoundManager) by [Tim Geyssens](https://twitter.com/timgeyssens) and was created with his permission. The code base used as a point of reference was taken prior to the Licesencing Model change.

## Credits

The logo uses [404](https://thenounproject.com/term/404/3283006 ) from the [Noun Project](https://thenounproject.com) by [Ilham Fitrotul Hayat](https://thenounproject.com/fhilham), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).
