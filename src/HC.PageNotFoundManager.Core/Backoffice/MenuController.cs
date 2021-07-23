using HC.PageNotFoundManager.Core.Caching;
using HC.PageNotFoundManager.Core.Config;
using HC.PageNotFoundManager.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace HC.PageNotFoundManager.Core.Backoffice
{
    [PluginController(Constants.BackOffice)]
    public class MenuController : UmbracoAuthorizedJsonController
    {
        private readonly DistributedCache distributedCache;
        private readonly IPageNotFoundConfig config;

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
