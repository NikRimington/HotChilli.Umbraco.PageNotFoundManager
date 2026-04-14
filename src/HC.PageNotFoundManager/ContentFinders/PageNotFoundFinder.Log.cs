using System;
using Microsoft.Extensions.Logging;

namespace HC.PageNotFoundManager.ContentFinders;

public partial class PageNotFoundFinder
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Processing request for {Uri} (culture: {Culture})")]
    private partial void LogRequestReceived(string uri, string? culture);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Path {Uri} excluded by rule \"{MatchedPath}\" (configured paths: [{ExcludePaths}])")]
    private partial void LogExcludedPath(string uri, string matchedPath, string excludePaths);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: No domain on request, resolved start node ID: {StartNodeId}")]
    private partial void LogDomainResolved(int? startNodeId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Domain from request: {DomainName}, content ID: {ContentId}")]
    private partial void LogDomainFromRequest(string domainName, int? contentId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Domain matched: {DomainName} (root content ID: {RootContentId})")]
    private partial void LogDomainMatched(string domainName, int? rootContentId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: No domain matched from {DomainCount} configured domain(s)")]
    private partial void LogNoDomainMatched(int domainCount);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: No domains configured in Umbraco")]
    private partial void LogNoDomainsConfigured();

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Route resolution for \"{OriginalUri}\": stripped to \"{StrippedUri}\" → {DocumentKey}")]
    private partial void LogRouteResolution(string originalUri, string strippedUri, Guid? documentKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "PageNotFoundManager: No content node found after stripping URI \"{Uri}\". Check that content is published and routes are up to date.")]
    private partial void LogNoContentNodeFound(string uri);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Closest content node: \"{Name}\" ({Key})")]
    private partial void LogContentNodeFound(string name, Guid key);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Config for {NodeKey}: Explicit404={Explicit404}, Inherited404={Inherited404}, resolved key={ResolvedKey}")]
    private partial void LogNotFoundConfig(Guid nodeKey, Guid? explicit404, Guid? inherited404, Guid resolvedKey);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Walking to parent: \"{Name}\" ({Key})")]
    private partial void LogWalkingToParent(string name, Guid key);

    [LoggerMessage(Level = LogLevel.Warning, Message = "PageNotFoundManager: No 404 page configured for any ancestor of \"{Uri}\"")]
    private partial void LogNo404PageConfigured(string uri);

    [LoggerMessage(Level = LogLevel.Information, Message = "PageNotFoundManager: Serving 404 page \"{Name}\" ({Key}) for \"{Uri}\"")]
    private partial void LogServing404Page(string name, Guid key, string uri);

    [LoggerMessage(Level = LogLevel.Error, Message = "PageNotFoundManager: Error finding content for {RequestUri}")]
    private partial void LogError(Exception ex, Uri requestUri);
}
