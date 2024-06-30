using System;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Models;
using HC.PageNotFoundManager.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Serialization;

namespace HC.PageNotFoundManager.Caching
{
    public class PageNotFoundCacheRefresher : PayloadCacheRefresherBase<CacheRefresherNotification, PageNotFoundRequest>
    {
        public const string Id = "fb984dde-7b8b-4e58-bb5c-5f0cb043af4e";

        private readonly IPageNotFoundConfig config;

        private readonly ICacheService cacheService;

        public PageNotFoundCacheRefresher(
            AppCaches appCaches,
            IPageNotFoundConfig config,
            IJsonSerializer jsonSerializer,
            IEventAggregator eventAggregator,
            ICacheRefresherNotificationFactory cacheRefresherNotificationFactory, ICacheService cacheService)
            : base(appCaches, jsonSerializer, eventAggregator, cacheRefresherNotificationFactory)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.cacheService = cacheService;
        }

        public override string Name => "PageNotFoundManager Cache Refresher";

        public override Guid RefresherUniqueId => new(Id);

        public override void Refresh(PageNotFoundRequest[] payloads)
        {
            foreach (var payload in payloads)
            {
                config.SetNotFoundPage(payload.ParentId, payload.NotFoundPageId, false);
            }

            cacheService.RefreshCache();
        }
    }
}