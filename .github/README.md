
# Page Not Found Manager

![Logo](https://raw.githubusercontent.com/NikRimington/HotChilli.Umbraco.PageNotFoundManager/develop/docs/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/HotChilli.Umbraco.PageNotFound.svg)](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/)
[![Our Umbraco](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.com/packages/backoffice-extensions/hot-chilli-page-not-found-manager/)

## Supported Umbraco Versions

- **Version 2.x**: Supports Umbraco 9–13
- **Version 3.x (beta)**: Supports Umbraco 14 *(see note below)*
- **Version 16.x**: Supports Umbraco 16

> **Note:** There is also a `dev/umbv15` branch that targets Umbraco 15. This version was never officially released but should work if needed.

## Installation

Install the package from [NuGet](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/):

```powershell
# For Umbraco 9–13
PM> Install-Package HotChilli.Umbraco.PageNotFound -Version 2.*

# For Umbraco 14 (beta)
PM> Install-Package HotChilli.Umbraco.PageNotFound -Version 3.*-beta

# For Umbraco 16
PM> Install-Package HotChilli.Umbraco.PageNotFound -Version 16.*
```

## Usage

Once installed, Page Not Found Manager is loaded via an Umbraco Composer by default. For advanced scenarios, you can use the `UsePageNotFoundManager` extension method on `IUmbracoBuilder` to control when it loads.

After installation, the package extends the "Do Something Else" context menu in the Umbraco backoffice, allowing you to pick a 404 page from a content node. This enables multiple 404 pages for different website sections.

> *Improved documentation will come in time. PRs and suggestions are welcome!*

## Development Branches

- `dev/v2`: Umbraco 9–13 (stable)
- `dev/v3`: Umbraco 14 (beta)
- `dev/umbv15`: Umbraco 15 (unreleased, but should work)
- `dev/umbv16`: Umbraco 16 (stable)

Sample/test sites for each version can be found in the `TestSites` directory.

## Future Plans

There are plans to investigate serving custom 404 pages for missing media items. Currently, IIS falls back to its default 404 page for missing media. Suggestions and PRs are welcome!

## Special Thanks

This package is a port of [PageNotFoundManager](https://github.com/TimGeyssens/UmbracoPageNotFoundManager) by [Tim Geyssens](https://twitter.com/timgeyssens), created with his permission. The code base used as a reference was taken prior to the licensing model change.

## Credits

The logo uses [404](https://thenounproject.com/term/404/3283006) from the [Noun Project](https://thenounproject.com) by [Ilham Fitrotul Hayat](https://thenounproject.com/fhilham), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).
