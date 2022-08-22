using System;
using HC.PageNotFoundManager.Caching;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace HC.PageNotFoundManager.Backoffice
{
    [PluginController(Constants.Constants.BackOffice)]
    public class MenuController : UmbracoAuthorizedJsonController
    {
        private readonly IPageNotFoundConfig config;

        private readonly DistributedCache distributedCache;

        public MenuController(DistributedCache distributedCache, IPageNotFoundConfig config)
        {
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public int GetNotFoundPage(int pageId)
        {
            return config.GetNotFoundPage(pageId);
        }

        [HttpPost]
        public void SetNotFoundPage(PageNotFoundRequest request)
        {
            config.SetNotFoundPage(request.ParentId, request.NotFoundPageId, true);

            distributedCache.RefreshPageNotFoundConfig(request);
        }
    }
}