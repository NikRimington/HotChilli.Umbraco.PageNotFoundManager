using HCS.PageNotFoundManager.Core.Models;
using System;
using Umbraco.Cms.Core.Cache;

namespace HCS.PageNotFoundManager.Core.Caching
{
    public static class DistributedCacheExtensions
    {
        public static void RefreshPageNotFoundConfig(this DistributedCache dc, PageNotFoundRequest pageNotFound)
        {
            dc.RefreshByPayload(new Guid(PageNotFoundCacheRefresher.Id), new[] { pageNotFound });
        }
    }

}