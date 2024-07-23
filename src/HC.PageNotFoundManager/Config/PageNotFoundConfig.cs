using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Config;

public class PageNotFoundConfig : IPageNotFoundConfig
{
    private const string CacheKey = "PageNotFoundConfig";

    private readonly IAppPolicyCache appPolicyCache;

    private readonly IScopeProvider scopeProvider;

    private readonly IUmbracoContextFactory umbracoContextFactory;

    public PageNotFoundConfig(
        IScopeProvider scopeProvider,
        IUmbracoContextFactory umbracoContextFactory,
        IAppPolicyCache appPolicyCache)
    {
        this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        this.umbracoContextFactory =
            umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
        this.appPolicyCache = appPolicyCache ?? throw new ArgumentNullException(nameof(appPolicyCache));
    }

    private List<Models.PageNotFound> ConfiguredPages
    {
        get
        {
            var us = (List<Models.PageNotFound>)appPolicyCache.Get(CacheKey, LoadFromDb);
            return us;
        }
    }

    public Guid? GetNotFoundPage(int parentId)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
        var parentNode = umbracoContext.UmbracoContext.Content?.GetById(parentId);
        return parentNode != null ? GetNotFoundPage(parentNode.Key) : null;
    }

    public Guid? GetNotFoundPage(Guid parentKey)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var x = ConfiguredPages.FirstOrDefault(p => p.ParentId == parentKey);
        return x?.NotFoundPageId;
    }

    public void RefreshCache()
    {
        appPolicyCache.ClearByKey(CacheKey);
        appPolicyCache.Insert(CacheKey, LoadFromDb);
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
        using (var scope = scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var page = db.Query<Models.PageNotFound>().Where(p => p.ParentId == parentKey).FirstOrDefault();
            if (page == null && !Guid.Empty.Equals(pageNotFoundKey))
            {
                // create the page
                db.Insert(new Models.PageNotFound { ParentId = parentKey, NotFoundPageId = pageNotFoundKey });
            }
            else if (page != null)
            {
                if (Guid.Empty.Equals(pageNotFoundKey))
                {
                    db.Delete(page);
                }
                else
                {
                    // update the existing page
                    page.NotFoundPageId = pageNotFoundKey;
                    db.Update(Models.PageNotFound.TableName, "ParentId", page);
                }
            }

            scope.Complete();
        }

        if (refreshCache)
        {
            RefreshCache();
        }
    }

    private List<Models.PageNotFound> LoadFromDb()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = scope.SqlContext.Sql().Select("*").From<Models.PageNotFound>();
        var pages = scope.Database.Fetch<Models.PageNotFound>(sql);
        scope.Complete();
        return pages;
    }
}
