using System;
using System.Linq;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Extensions;
using HC.PageNotFoundManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;

namespace HC.PageNotFoundManager.ContentFinders;

public partial class PageNotFoundFinder : IContentLastChanceFinder
{
    private readonly IPageNotFoundService config;
    private readonly PageNotFoundManagerSettings pageNotFoundManagerSettings;

    private readonly ILogger<PageNotFoundFinder> logger;

    private readonly IDomainService domainService;

    private readonly IUmbracoContextFactory umbracoContextFactory;
    private readonly IDocumentUrlService documentUrlService;
    private readonly IDocumentNavigationQueryService documentNavigationQueryService;

    public PageNotFoundFinder(
        IDomainService domainService,
        IUmbracoContextFactory umbracoContextFactory,
        IDocumentUrlService documentUrlService,
        IDocumentNavigationQueryService documentNavigationQueryService,
        IPageNotFoundService config,
        IOptions<PageNotFoundManagerSettings> pageNotFoundManagerSettings,
        ILogger<PageNotFoundFinder> logger)
    {
        this.domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        this.umbracoContextFactory =
            umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
        this.documentUrlService = documentUrlService;
        this.documentNavigationQueryService = documentNavigationQueryService;
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.pageNotFoundManagerSettings = pageNotFoundManagerSettings.Value ?? throw new ArgumentNullException(nameof(pageNotFoundManagerSettings));
        this.logger = logger;
    }

    public async Task<bool> TryFindContent(IPublishedRequestBuilder request)
    {
        try
        {
            string uri = request.AbsolutePathDecoded;
            // a route is "/path/to/page" when there is no domain, and "123/path/to/page" when there is a domain, and then 123 is the ID of the node which is the root of the domain
            //get domain name from Uri
            // find umbraco home node for uri's domain, and get the id of the node it is set on

            LogRequestReceived(uri, request.Culture);

            var excludePaths = pageNotFoundManagerSettings.ExcludePaths;
            if (excludePaths.Any(excludePath => uri.StartsWith(excludePath, StringComparison.OrdinalIgnoreCase)))
            {
                var matchedPath = excludePaths.First(p => uri.StartsWith(p, StringComparison.OrdinalIgnoreCase));
                LogExcludedPath(uri, matchedPath, string.Join(", ", excludePaths.Select(p => $"\"{p}\"")));
                return false;
            }

            int? documentStartNodeId;
            if (request.Domain is null)
            {
                documentStartNodeId = await GetDomain(request);
                LogDomainResolved(documentStartNodeId);
            }
            else
            {
                documentStartNodeId = request.Domain.ContentId;
                LogDomainFromRequest(request.Domain.Name, documentStartNodeId);
            }

            using var umbracoContext = umbracoContextFactory.EnsureUmbracoContext();

            var documentKey = documentUrlService.GetDocumentKeyByRoute(uri, request.Culture, documentStartNodeId, false);

            while (documentKey == null && uri.Length > 0)
            {
                uri = uri.Remove(uri.Length - 1, 1);
                documentKey = documentUrlService.GetDocumentKeyByRoute(uri, request.Culture, documentStartNodeId, false);
            }

            LogRouteResolution(request.AbsolutePathDecoded, uri, documentKey);

            var contentNode = documentKey != null ? umbracoContext.UmbracoContext.Content.GetById(documentKey!.Value) : null;
            if (contentNode == null)
            {
                LogNoContentNodeFound(request.AbsolutePathDecoded);
                return false;
            }

            LogContentNodeFound(contentNode.Name, contentNode.Key);

            var nfp = config.GetNotFoundPage(documentKey!.Value);
            var nfpKey = nfp?.Explicit404 ?? nfp?.Inherited404?.Explicit404 ?? Guid.Empty;

            LogNotFoundConfig(contentNode.Key, nfp?.Explicit404, nfp?.Inherited404?.Explicit404, nfpKey);

            var content = umbracoContext.UmbracoContext.Content.GetById(nfpKey);

            while (content == null && contentNode.TryGetParent(documentNavigationQueryService,
                umbracoContext.UmbracoContext.Content, out var parent) && parent != null)
            {
                contentNode = parent;
                LogWalkingToParent(contentNode.Name, contentNode.Key);

                nfp = config.GetNotFoundPage(contentNode.Key);
                nfpKey = nfp?.Explicit404 ?? nfp?.Inherited404?.Explicit404 ?? Guid.Empty;

                LogNotFoundConfig(contentNode.Key, nfp?.Explicit404, nfp?.Inherited404?.Explicit404, nfpKey);

                content = umbracoContext.UmbracoContext.Content.GetById(nfpKey);
            }

            if (content == null)
            {
                LogNo404PageConfigured(request.AbsolutePathDecoded);
                return false;
            }

            LogServing404Page(content.Name, content.Key, request.AbsolutePathDecoded);
            request.SetResponseStatus(404);
            request.SetPublishedContent(content);
            return true;
        }
        catch (Exception ex)
        {
            LogError(ex, request.Uri);
            return false;
        }
    }

    private static bool CurrentRequestStartsWithDomainAndDomainDoesntStartWithHttp(
        IPublishedRequestBuilder request,
        IDomain currentDomain)
    {
        return (request.Uri.Authority.ToLower() + request.Uri.AbsolutePath.ToLower()).StartsWith(
            currentDomain.DomainName.ToLower(), StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsCurrentDomainIsAbsoluteAndCurrentRequestStartsWithDomain(
        IPublishedRequestBuilder request,
        IDomain currentDomain)
    {
        return currentDomain.DomainName.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
               && request.Uri.AbsoluteUri.StartsWith(currentDomain.DomainName.ToLower(), StringComparison.InvariantCultureIgnoreCase);
    }

    //Does not handle relative Domains.
    private async Task<int?> GetDomain(IPublishedRequestBuilder request)
    {
        var domains = (await domainService.GetAllAsync(true)).ToList();

        if (domains.Count != 0)
        {
            //A domain can be defined with or without http(s) so we neet to check for both cases.
            IDomain? domain = null;
            foreach (var currentDomain in domains)
            {
                if (IsCurrentDomainIsAbsoluteAndCurrentRequestStartsWithDomain(request, currentDomain)
                    || CurrentRequestStartsWithDomainAndDomainDoesntStartWithHttp(request, currentDomain))
                {
                    domain = currentDomain;
                    break;
                }
            }

            if (domain != null)
            {
                LogDomainMatched(domain.DomainName, domain.RootContentId);
                return domain.RootContentId;
            }

            LogNoDomainMatched(domains.Count);
        }
        else
        {
            LogNoDomainsConfigured();
        }

        return null;
    }
}
