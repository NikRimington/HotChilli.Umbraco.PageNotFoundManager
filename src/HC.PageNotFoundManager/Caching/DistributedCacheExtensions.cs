using System;
using HC.PageNotFoundManager.Models;
using Umbraco.Cms.Core.Cache;

namespace HC.PageNotFoundManager.Caching;

public static class DistributedCacheExtensions
{
    public static void RefreshPageNotFoundConfig(this DistributedCache dc, PageNotFoundRequest pageNotFound)
    {
        dc.RefreshByPayload(new Guid(PageNotFoundCacheRefresher.Id), new[] { pageNotFound });
    }
}