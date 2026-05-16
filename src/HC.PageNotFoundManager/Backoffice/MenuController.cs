using System;
using HC.PageNotFoundManager.Caching;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace HC.PageNotFoundManager.Backoffice
{
    [PluginController(Constants.Constants.BackOffice)]
    public class MenuController : UmbracoAuthorizedJsonController
    {
        private readonly IPageNotFoundConfig config;

        private readonly DistributedCache distributedCache;

        private readonly IRelationService relationService;

        private readonly ILogger<MenuController> logger;

        public MenuController(
            DistributedCache distributedCache,
            IPageNotFoundConfig config,
            IRelationService relationService,
            ILogger<MenuController> logger)
        {
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.relationService = relationService ?? throw new ArgumentNullException(nameof(relationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int GetNotFoundPage(int pageId)
        {
            return config.GetNotFoundPage(pageId);
        }

        [HttpPost]
        public void SetNotFoundPage(PageNotFoundRequest request)
        {
            config.SetNotFoundPage(request.ParentId, request.NotFoundPageId, true);

            SyncRelation(request.ParentId, request.NotFoundPageId);

            distributedCache.RefreshPageNotFoundConfig(request);
        }

        private void SyncRelation(int parentId, int notFoundPageId)
        {
            var existing = relationService.GetByParentId(parentId, Constants.Constants.RelationTypeAlias);
            if (existing != null)
            {
                foreach (var rel in existing)
                    relationService.Delete(rel);
            }

            if (notFoundPageId == 0)
                return;

            var relationType = relationService.GetRelationTypeByAlias(Constants.Constants.RelationTypeAlias);
            if (relationType is null)
            {
                logger.LogWarning("Page Not Found Manager relation type '{Alias}' not found; skipping relation creation.", Constants.Constants.RelationTypeAlias);
                return;
            }

            relationService.Relate(parentId, notFoundPageId, relationType);
        }
    }
}