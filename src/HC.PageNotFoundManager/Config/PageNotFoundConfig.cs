using System;
using System.Collections.Generic;
using System.Linq;
using HC.PageNotFoundManager.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;

namespace HC.PageNotFoundManager.Config
{
    public class PageNotFoundConfig : IPageNotFoundConfig
    {
        private const string CacheKey = "PageNotFoundConfig";

        private readonly IAppPolicyCache appPolicyCache;

        private readonly IScopeProvider scopeProvider;

        private readonly IUmbracoContextFactory umbracoContextFactory;

        private readonly IDatabaseService databaseService;

        private readonly ICacheService cacheService;

        public PageNotFoundConfig(
            IScopeProvider scopeProvider,
            IUmbracoContextFactory umbracoContextFactory,
            IAppPolicyCache appPolicyCache, IDatabaseService databaseService, ICacheService cacheService)
        {
            this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            this.umbracoContextFactory =
                umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
            this.appPolicyCache = appPolicyCache ?? throw new ArgumentNullException(nameof(appPolicyCache));
            this.databaseService = databaseService;
            this.cacheService = cacheService;
        }

        private List<Models.PageNotFound> ConfiguredPages
        {
            get
            {
                var us = (List<Models.PageNotFound>)appPolicyCache.Get(CacheKey, LoadFromDb);
                return us;
            }
        }

        public int GetNotFoundPage(int parentId)
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
            var parentNode = umbracoContext.UmbracoContext.Content?.GetById(parentId);
            return parentNode != null ? GetNotFoundPage(parentNode.Key) : 0;
        }

        public int GetNotFoundPage(Guid parentKey)
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();

            var x = ConfiguredPages.FirstOrDefault(p => p.ParentId == parentKey);
            var page = x != null ? umbracoContext.UmbracoContext.Content?.GetById(x.NotFoundPageId) : null;
            return page != null ? page.Id : 0;
        }

        public void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache)
        {
            using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
            var parentPage = umbracoContext.UmbracoContext.Content?.GetById(parentId);
            var pageNotFoundPage = umbracoContext.UmbracoContext.Content?.GetById(pageNotFoundId);
            SetNotFoundPage(parentPage.Key, pageNotFoundPage != null ? pageNotFoundPage.Key : Guid.Empty, refreshCache);
        }

        public void SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache)
        {
            databaseService.InsertToDb(parentKey, pageNotFoundKey);

            if (refreshCache)
            {
                cacheService.RefreshCache();
            }
        }

        private IEnumerable<Models.PageNotFound> LoadFromDb()
        {
            var pages = databaseService.LoadFromDb();
            return pages;
        }
    }

    public interface IPageNotFoundConfig
    {
        int GetNotFoundPage(int parentId);

        int GetNotFoundPage(Guid parentKey);

        void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache);

        void SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache);
    }
}