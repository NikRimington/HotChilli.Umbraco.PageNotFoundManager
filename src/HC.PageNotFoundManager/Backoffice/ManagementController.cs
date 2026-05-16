using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using HC.PageNotFoundManager.Caching;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Filters;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Services;
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

    private readonly IRelationService relationService;

    private readonly IContentService contentService;

    private readonly ILogger<ManagementController> logger;

    public ManagementController(
        DistributedCache distributedCache,
        IPageNotFoundService service,
        IRelationService relationService,
        IContentService contentService,
        ILogger<ManagementController> logger)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        this.service = service ?? throw new ArgumentNullException(nameof(service));
        this.relationService = relationService ?? throw new ArgumentNullException(nameof(relationService));
        this.contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        SyncRelation(request.ParentId, request.NotFoundPageId ?? Guid.Empty);

        distributedCache.RefreshPageNotFoundConfig(request);

        return res;
    }

    private void SyncRelation(Guid parentKey, Guid notFoundPageKey)
    {
        var parent = contentService.GetById(parentKey);
        if (parent is null) return;

        var existing = relationService.GetByParentId(parent.Id, Constants.Constants.RelationTypeAlias);
        foreach (var rel in existing ?? [])
            relationService.Delete(rel);

        if (notFoundPageKey == Guid.Empty) return;

        var relationType = relationService.GetRelationTypeByAlias(Constants.Constants.RelationTypeAlias);
        if (relationType is null)
        {
            logger.LogWarning("Page Not Found Manager relation type '{Alias}' not found; skipping relation creation.", Constants.Constants.RelationTypeAlias);
            return;
        }

        var notFoundPage = contentService.GetById(notFoundPageKey);
        if (notFoundPage is null) return;

        relationService.Relate(parent.Id, notFoundPage.Id, relationType);
    }
}