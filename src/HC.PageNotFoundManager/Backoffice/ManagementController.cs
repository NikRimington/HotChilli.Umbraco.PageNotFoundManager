using System;
using Asp.Versioning;
using HC.PageNotFoundManager.Caching;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Filters;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Filters;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Web.Common.Authorization;

namespace HC.PageNotFoundManager.Backoffice;

[ApiController]
[ApiVersion("1.0")]
[MapToApi("hcs")]
[ApiExplorerSettings(GroupName = Constants.Constants.Name)]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[Route("api/v{version:apiVersion}/hcs")]
[AppendEventMessages]
[Produces("application/json")]
public class ManagementController : Controller
{
    private readonly IPageNotFoundConfig config;

    private readonly DistributedCache distributedCache;

    public ManagementController(DistributedCache distributedCache, IPageNotFoundConfig config)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    [HttpGet("get-not-found")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(Guid?), StatusCodes.Status200OK)]
    public Guid? GetNotFoundPage(Guid pageId)
    {
        return config.GetNotFoundPage(pageId);
    }

    [HttpPost("set-not-found")]
    [MapToApiVersion("1.0")]
    public void SetNotFoundPage(PageNotFoundRequest request)
    {
        config.SetNotFoundPage(request.ParentId, request.NotFoundPageId ?? Guid.Empty, true);

        distributedCache.RefreshPageNotFoundConfig(request);
    }
}