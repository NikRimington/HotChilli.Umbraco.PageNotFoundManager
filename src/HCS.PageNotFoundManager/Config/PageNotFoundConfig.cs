using HCS.PageNotFoundManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace HCS.PageNotFoundManager.Core.Config
{
    public class PageNotFoundConfig : IPageNotFoundConfig
    {
        private const string CacheKey = "PageNotFoundConfig";
        private readonly IScopeProvider scopeProvider;
        private readonly IUmbracoContextFactory umbracoContextFactory;
        private readonly IAppPolicyCache appPolicyCache;

        public PageNotFoundConfig(IScopeProvider scopeProvider, IUmbracoContextFactory umbracoContextFactory, IAppPolicyCache appPolicyCache)
        {
            this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            this.umbracoContextFactory = umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
            this.appPolicyCache = appPolicyCache ?? throw new ArgumentNullException(nameof(appPolicyCache));
        }

        public int GetNotFoundPage(int parentId)
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
            var parentNode = umbracoContext.UmbracoContext.Content.GetById(parentId);
            return parentNode != null ? GetNotFoundPage(parentNode.Key) : 0;

        }

        public int GetNotFoundPage(Guid parentKey)
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();

            var x = ConfiguredPages.FirstOrDefault(p => p.ParentId == parentKey);
            var page = x != null ? umbracoContext.UmbracoContext.Content.GetById(x.NotFoundPageId) : null;
            return page != null ? page.Id : 0;

        }

        public void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache)
        {
            using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
            {
                var parentPage = umbracoContext.UmbracoContext.Content.GetById(parentId);
                var pageNotFoundPage = umbracoContext.UmbracoContext.Content.GetById(pageNotFoundId);
                SetNotFoundPage(parentPage.Key, pageNotFoundPage != null ? pageNotFoundPage.Key : Guid.Empty, refreshCache);
            }
        }

        public void SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache)
        {

            using (var scope = scopeProvider.CreateScope())
            {
                var db = scope.Database;
                var page = db.Query<PageNotFound>().Where(p => p.ParentId == parentKey).FirstOrDefault();
                if (page == null && !Guid.Empty.Equals(pageNotFoundKey))
                {
                    // create the page
                    db.Insert(new PageNotFound { ParentId = parentKey, NotFoundPageId = pageNotFoundKey });
                }
                else if (page != null)
                {
                    if (Guid.Empty.Equals(pageNotFoundKey))
                        db.Delete(page);
                    else
                    {
                        // update the existing page
                        page.NotFoundPageId = pageNotFoundKey;
                        db.Update(PageNotFound.TableName, "ParentId", page);
                    }
                }
                scope.Complete();
            }

            if (refreshCache)
                RefreshCache();
        }

        public void RefreshCache()
        {
            appPolicyCache.ClearByKey(CacheKey);
            appPolicyCache.Insert(CacheKey, LoadFromDb);
        }

        private List<PageNotFound> ConfiguredPages
        {
            get
            {
                var us = (List<PageNotFound>)appPolicyCache.Get(CacheKey, LoadFromDb);
                return us;
            }
        }

        private List<PageNotFound> LoadFromDb()
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            var sql = scope.SqlContext.Sql().Select("*").From<PageNotFound>();
            var pages = scope.Database.Fetch<PageNotFound>(sql);
            scope.Complete();
            return pages;
        }
    }

    public interface IPageNotFoundConfig
    {
        int GetNotFoundPage(int parentId);
        int GetNotFoundPage(Guid parentKey);
        void RefreshCache();
        void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache);
        void SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache);
    }
}
