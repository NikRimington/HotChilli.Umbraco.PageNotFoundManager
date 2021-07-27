# Page Not Found Manager

<img src="docs/img/logo.png?raw=true" alt="Page not found" width="250" align="right" />

[![NuGet](https://img.shields.io/nuget/v/HotChilli.Umbraco.PageNotFound.svg)](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/)
[![Our Umbraco](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.com/packages/backoffice-extensions/hot-chilli-page-not-found-manager/)

## Getting Started

**This is currently a beta package**

Page Not Found Manager supports Umbraco v9 RC1+.

Umbraco v9 Packages are only available via Nuget, although there is a page on [Our.Umbraco.com](https://our.umbraco.com/packages/backoffice-extensions/hot-chilli-page-not-found-manager/) to aid discoverability.

## Installation

To [install from NuGet](https://www.nuget.org/packages/HotChilli.Umbraco.PageNotFound/), run the following command in your instance of Visual Studio.

    PM> Install-Package HotChilli.Umbraco.PageNotFound

## How to use

*Improved documentation will come in time*

Once the package has been installed there are two approaches to configuration. Out of the box, it will be loaded via an Umbraco Composer, but if you wish to have more control over when it loads, there is an extension method for `IUmbracoBuilder` called `UsePageNotFoundManager`.

Once installed and running, Page Not Found Manager extends the "Do Something Else" context menu with a new entry for picking a 404 page off of a content node.

This allows for multiple different 404 pages to be configured for different parts of the website.

## Future Plans

There is currently a plan to investigate and see if this same 404 page can be served for missing media items. In IIS, this would fall back to the default 404 page from IIS but I'm yet to understand this pipeline and if this is possible - I'm open to PR's and suggestions on improvements as well.

## Special Thanks

This package is a port of [PageNotFoundManager](https://github.com/TimGeyssens/UmbracoPageNotFoundManager) by [Tim Geyssens](https://twitter.com/timgeyssens) and was created with his permission. The code based used as a point of reference was taken prior to the Licesencing Model change.

## Credits

The logo uses [404](https://thenounproject.com/term/404/3283006 ) from the [Noun Project](https://thenounproject.com) by [Ilham Fitrotul Hayat](https://thenounproject.com/fhilham), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).
