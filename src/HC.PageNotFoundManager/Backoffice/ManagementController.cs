using System;
using System.Threading.Tasks;
using Asp.Versioning;
using HC.PageNotFoundManager.Caching;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Filters;
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
    private readonly IPageNotFoundService service;

    private readonly DistributedCache distributedCache;

    public ManagementController(DistributedCache distributedCache, IPageNotFoundService service)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        this.service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet("get-not-found")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(PageNotFoundDetails), StatusCodes.Status200OK)]
    public PageNotFoundDetails? GetNotFoundPage(Guid pageId)
    {
        return service.GetNotFoundPage(pageId);
    }

    [HttpPost("set-not-found")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(PageNotFoundDetails), StatusCodes.Status200OK)]
    public async Task<PageNotFoundDetails> SetNotFoundPage(PageNotFoundRequest request)
    {
        var res = await service.SetNotFoundPage(request.ParentId, request.NotFoundPageId ?? Guid.Empty, true);

        distributedCache.RefreshPageNotFoundConfig(request);

        return res;
    }
}