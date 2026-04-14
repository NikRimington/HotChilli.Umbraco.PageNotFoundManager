using System;
using Microsoft.Extensions.Logging;

namespace HC.PageNotFoundManager.Config;

public partial class PageNotFoundConfigService
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Config lookup by ID {NodeId} — node not found in content cache")]
    private partial void LogNodeNotFoundById(int nodeId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Config lookup by key {NodeKey} — node not found in content cache")]
    private partial void LogNodeNotFoundByKey(Guid nodeKey);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Config found for {NodeKey} (\"{NodeName}\"): Explicit404={Explicit404}")]
    private partial void LogConfigFound(Guid nodeKey, string nodeName, Guid? explicit404);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: No explicit config for {NodeKey} (\"{NodeName}\"), will check ancestors")]
    private partial void LogNoConfigForNode(Guid nodeKey, string nodeName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Cache refreshed")]
    private partial void LogCacheRefresh();

    [LoggerMessage(Level = LogLevel.Information, Message = "PageNotFoundManager: Setting 404 page for parent {ParentKey} to {PageNotFoundKey}")]
    private partial void LogSettingNotFoundPage(Guid parentKey, Guid pageNotFoundKey);

    [LoggerMessage(Level = LogLevel.Information, Message = "PageNotFoundManager: Created 404 config for {ParentKey} → {PageNotFoundKey}")]
    private partial void LogConfigCreated(Guid parentKey, Guid pageNotFoundKey);

    [LoggerMessage(Level = LogLevel.Information, Message = "PageNotFoundManager: Deleted 404 config for {ParentKey}")]
    private partial void LogConfigDeleted(Guid parentKey);

    [LoggerMessage(Level = LogLevel.Information, Message = "PageNotFoundManager: Updated 404 config for {ParentKey} → {PageNotFoundKey}")]
    private partial void LogConfigUpdated(Guid parentKey, Guid pageNotFoundKey);

    [LoggerMessage(Level = LogLevel.Debug, Message = "PageNotFoundManager: Loaded {Count} 404 config(s) from database")]
    private partial void LogLoadedFromDb(int count);
}
