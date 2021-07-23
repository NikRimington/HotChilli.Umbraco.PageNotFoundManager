using HC.PageNotFoundManager.Core.Config;
using HC.PageNotFoundManager.Core.Models;
using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Serialization;

namespace HC.PageNotFoundManager.Core.Caching
{
    public class PageNotFoundCacheRefresher : PayloadCacheRefresherBase<CacheRefresherNotification, PageNotFoundRequest>
    {
        public const string Id = "fb984dde-7b8b-4e58-bb5c-5f0cb043af4e";
        private readonly IPageNotFoundConfig config;

        public PageNotFoundCacheRefresher(AppCaches appCaches, IPageNotFoundConfig config,
            IJsonSerializer jsonSerializer, IEventAggregator eventAggregator, ICacheRefresherNotificationFactory cacheRefresherNotificationFactory) :
            base(appCaches, jsonSerializer, eventAggregator, cacheRefresherNotificationFactory)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public override string Name
        {
            get { return "PageNotFoundManager Cache Refresher"; }
        }

        public override Guid RefresherUniqueId => new(Id);


        public override void Refresh(PageNotFoundRequest[] payloads)
        {
            foreach (var payload in payloads)
                config.SetNotFoundPage(payload.ParentId, payload.NotFoundPageId, false);
            config.RefreshCache();
        }

    }

}