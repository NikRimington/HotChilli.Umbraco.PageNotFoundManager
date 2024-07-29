using HC.PageNotFoundManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Config;

public class PageNotFoundConfigService : IPageNotFoundService
{
    private const string CacheKey = "PageNotFoundConfig";

    private readonly IAppPolicyCache appPolicyCache;

    private readonly IScopeProvider scopeProvider;

    private readonly IUmbracoContextFactory umbracoContextFactory;

    public PageNotFoundConfigService(
        IScopeProvider scopeProvider,
        IUmbracoContextFactory umbracoContextFactory,
        IAppPolicyCache appPolicyCache)
    {
        this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        this.umbracoContextFactory =
            umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
        this.appPolicyCache = appPolicyCache ?? throw new ArgumentNullException(nameof(appPolicyCache));
    }

    private List<PageNotFoundDetails> ConfiguredPages
    {
        get
        {
            var us = (List<PageNotFoundDetails>?)appPolicyCache.Get(CacheKey, LoadFromDb);
            return us ?? [];
        }
    }

    public PageNotFoundDetails? GetNotFoundPage(int nodeId)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
        var node = umbracoContext.UmbracoContext.Content?.GetById(nodeId);
        return node != null ? GetNotFoundPage(node, true) : null;
    }

    public PageNotFoundDetails? GetNotFoundPage(Guid nodeKey)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
        var node = umbracoContext.UmbracoContext.Content?.GetById(nodeKey);
        
        return node != null ? GetNotFoundPage(node, true) : null;
    }

    private PageNotFoundDetails? GetNotFoundPage(IPublishedContent node, bool fetchInherited)
    {
        var x = ConfiguredPages.FirstOrDefault(p => p.PageId == node.Key);
        x ??= new PageNotFoundDetails
            {
                PageId = node.Key,
            };
        if(fetchInherited)
            x.Inherited404 = GetAncestor404(node.Parent);
        return x;
    }

    public void RefreshCache()
    {
        appPolicyCache.ClearByKey(CacheKey);
        appPolicyCache.Insert(CacheKey, LoadFromDb);
    }

    public async Task<PageNotFoundDetails> SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache)
    {
        using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
        var parentPage = umbracoContext.UmbracoContext.Content?.GetById(parentId);
        var pageNotFoundPage = umbracoContext.UmbracoContext.Content?.GetById(pageNotFoundId);
        return await SetNotFoundPage(parentPage?.Key ?? Guid.Empty, pageNotFoundPage != null ? pageNotFoundPage.Key : Guid.Empty, refreshCache);
    }

    public async Task<PageNotFoundDetails> SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache)
    {
        using (var scope = scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var page = db.Query<Models.DatabaseModels.PageNotFound>().Where(p => p.ParentId == parentKey).FirstOrDefault();
            if (page == null && !Guid.Empty.Equals(pageNotFoundKey))
            {
                // create the page
                await db.InsertAsync(new Models.DatabaseModels.PageNotFound { ParentId = parentKey, NotFoundPageId = pageNotFoundKey });
            }
            else if (page != null)
            {
                if (Guid.Empty.Equals(pageNotFoundKey))
                {
                    await db.DeleteAsync(page);
                }
                else
                {
                    // update the existing page
                    page.NotFoundPageId = pageNotFoundKey;
                    db.Update(Models.DatabaseModels.PageNotFound.TableName, "ParentId", page);
                }
            }

            scope.Complete();

            
        }

        if (refreshCache)
        {
            RefreshCache();
        }

        return new PageNotFoundDetails
        {
            PageId = parentKey,
            Explicit404 = pageNotFoundKey == Guid.Empty ? null : pageNotFoundKey,
            Inherited404 = pageNotFoundKey != Guid.Empty ? null : GetAncestor404(parentKey)
        };
    }

    private PageNotFoundDetails? GetAncestor404(Guid nodeKey)
    {
        using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
        var node = umbracoContext.UmbracoContext.Content?.GetById(nodeKey);
        return node == null ? null : GetAncestor404(node);
    }

    private PageNotFoundDetails? GetAncestor404(IPublishedContent? node)
    {
        if (node == null)
            return null;
        //TODO: Need some logic, we don't want the whole tree just the first inherited 404
        var ancestor404 = GetNotFoundPage(node, false);
        
        if((ancestor404 == null || !ancestor404.Has404()) && node.Parent != null)
            return GetAncestor404(node.Parent);
        return ancestor404;
    }

    private List<PageNotFoundDetails> LoadFromDb()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = scope.SqlContext.Sql().Select("*").From<Models.DatabaseModels.PageNotFound>();
        var pages = scope.Database.Fetch<Models.DatabaseModels.PageNotFound>(sql);
        scope.Complete();
        return pages.Select(p => new PageNotFoundDetails
        {
            PageId = p.ParentId,
            Explicit404 = p.NotFoundPageId == Guid.Empty ? null : p.NotFoundPageId
        }).ToList();
    }
}
