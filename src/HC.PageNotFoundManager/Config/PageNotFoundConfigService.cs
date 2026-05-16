using HC.PageNotFoundManager.Extensions;
using HC.PageNotFoundManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Config;

public partial class PageNotFoundConfigService : IPageNotFoundService
{
    private const string CacheKey = "PageNotFoundConfig";

    private readonly IAppPolicyCache appPolicyCache;
    private readonly IDocumentNavigationQueryService documentNavigationQueryService;
    private readonly IScopeProvider scopeProvider;
    private readonly IUmbracoContextFactory umbracoContextFactory;
    private readonly ILogger<PageNotFoundConfigService> logger;

    public PageNotFoundConfigService(
        IScopeProvider scopeProvider,
        IUmbracoContextFactory umbracoContextFactory,
        IAppPolicyCache appPolicyCache,
        IDocumentNavigationQueryService documentNavigationQueryService,
        ILogger<PageNotFoundConfigService> logger)
    {
        this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        this.umbracoContextFactory =
            umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
        this.appPolicyCache = appPolicyCache ?? throw new ArgumentNullException(nameof(appPolicyCache));
        this.documentNavigationQueryService = documentNavigationQueryService;
        this.logger = logger;
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

        if (node == null)
        {
            LogNodeNotFoundById(nodeId);
            return null;
        }

        return umbracoContext.UmbracoContext.Content != null
            ? GetNotFoundPage(node, true, umbracoContext.UmbracoContext.Content) : null;
    }

    public PageNotFoundDetails? GetNotFoundPage(Guid nodeKey)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();
        var node = umbracoContext.UmbracoContext.Content?.GetById(nodeKey);

        if (node == null)
        {
            LogNodeNotFoundByKey(nodeKey);
            return null;
        }

        return umbracoContext.UmbracoContext.Content != null
            ? GetNotFoundPage(node, true, umbracoContext.UmbracoContext.Content) : null;
    }

    private PageNotFoundDetails? GetNotFoundPage(IPublishedContent node, bool fetchInherited, IPublishedContentCache content)
    {
        var x = ConfiguredPages.FirstOrDefault(p => p.PageId == node.Key);

        if (x != null)
        {
            LogConfigFound(node.Key, node.Name, x.Explicit404);
        }
        else
        {
            LogNoConfigForNode(node.Key, node.Name);
        }

        x ??= new PageNotFoundDetails
            {
                PageId = node.Key,
            };
        if(fetchInherited)
            x.Inherited404 = GetAncestor404(node.TryGetParent(documentNavigationQueryService, content, out var parent) ? parent : null, content);
        return x;
    }

    public bool IsUsedAsNotFoundPage(Guid pageKey) =>
        ConfiguredPages.Any(p => p.Explicit404.HasValue && p.Explicit404.Value == pageKey);

    public void RefreshCache()
    {
        LogCacheRefresh();
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
        LogSettingNotFoundPage(parentKey, pageNotFoundKey);

        using (var scope = scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var page = db.Query<Models.DatabaseModels.PageNotFound>().Where(p => p.ParentId == parentKey).FirstOrDefault();
            if (page == null && !Guid.Empty.Equals(pageNotFoundKey))
            {
                // create the page
                await db.InsertAsync(new Models.DatabaseModels.PageNotFound { ParentId = parentKey, NotFoundPageId = pageNotFoundKey });
                LogConfigCreated(parentKey, pageNotFoundKey);
            }
            else if (page != null)
            {
                if (Guid.Empty.Equals(pageNotFoundKey))
                {
                    await db.DeleteAsync(page);
                    LogConfigDeleted(parentKey);
                }
                else
                {
                    // update the existing page
                    page.NotFoundPageId = pageNotFoundKey;
                    db.Update(Models.DatabaseModels.PageNotFound.TableName, "ParentId", page);
                    LogConfigUpdated(parentKey, pageNotFoundKey);
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
        return node != null && umbracoContext.UmbracoContext.Content != null
            ? GetAncestor404(node, umbracoContext.UmbracoContext.Content)
            : null;
    }

    private PageNotFoundDetails? GetAncestor404(IPublishedContent? node, IPublishedContentCache content)
    {
        if (node == null)
            return null;
        var ancestor404 = GetNotFoundPage(node, false, content);

        if((ancestor404 == null || !ancestor404.Has404()) && node.TryGetParent(documentNavigationQueryService, content, out var parent) && parent != null)
            return GetAncestor404(parent, content);
        return ancestor404;
    }

    private List<PageNotFoundDetails> LoadFromDb()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        var sql = scope.SqlContext.Sql().Select("*").From<Models.DatabaseModels.PageNotFound>();
        var pages = scope.Database.Fetch<Models.DatabaseModels.PageNotFound>(sql);
        scope.Complete();

        var result = pages.Select(p => new PageNotFoundDetails
        {
            PageId = p.ParentId,
            Explicit404 = p.NotFoundPageId == Guid.Empty ? null : p.NotFoundPageId
        }).ToList();

        LogLoadedFromDb(result.Count);
        return result;
    }
}
