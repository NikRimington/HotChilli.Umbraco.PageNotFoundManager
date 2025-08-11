using System;
using System.Linq;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.Extensions;
using HC.PageNotFoundManager.Models;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Cms.Core.Web;

namespace HC.PageNotFoundManager.ContentFinders;

public class PageNotFoundFinder : IContentLastChanceFinder
{
    private readonly IPageNotFoundService config;
    private readonly PageNotFoundManagerSettings pageNotFoundManagerSettings;

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
        IOptions<PageNotFoundManagerSettings> pageNotFoundManagerSettings)
    {
        this.domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        this.umbracoContextFactory =
            umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
        this.documentUrlService = documentUrlService;
        this.documentNavigationQueryService = documentNavigationQueryService;
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.pageNotFoundManagerSettings = pageNotFoundManagerSettings.Value ?? throw new ArgumentNullException(nameof(pageNotFoundManagerSettings));
    }

    public async Task<bool> TryFindContent(IPublishedRequestBuilder request)
    {
        string uri = request.AbsolutePathDecoded;
        // a route is "/path/to/page" when there is no domain, and "123/path/to/page" when there is a domain, and then 123 is the ID of the node which is the root of the domain
        //get domain name from Uri
        // find umbraco home node for uri's domain, and get the id of the node it is set on

        if (pageNotFoundManagerSettings.ExcludePaths.Any(excludePath => uri.StartsWith(excludePath, StringComparison.OrdinalIgnoreCase)))
            return false;

        var domains = (await domainService.GetAllAsync(true)).ToList();
        var domainRoutePrefixId = string.Empty;
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
                // the domain has a RootContentId that we can use as the prefix.
                domainRoutePrefixId = domain.RootContentId.ToString();
            }
        }

        using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
        {
            var documentKey = documentUrlService.GetDocumentKeyByRoute(domainRoutePrefixId + uri, request.Culture, null, false);            
            while (documentKey == null)
            {
                uri = uri.Remove(uri.Length - 1, 1);
                documentKey = documentUrlService.GetDocumentKeyByRoute(domainRoutePrefixId + uri, request.Culture, null, false);
            }

            var contentNode = documentKey != null ? umbracoContext.UmbracoContext.Content.GetById(documentKey!.Value) : null;

            var nfp = config.GetNotFoundPage(documentKey ?? Guid.Empty);

            var nfpKey = nfp?.Explicit404 ?? nfp?.Inherited404?.Explicit404 ?? Guid.Empty;
            var content = umbracoContext.UmbracoContext.Content.GetById(nfpKey);

            while (content == null && contentNode.TryGetParent(documentNavigationQueryService, 
                umbracoContext.UmbracoContext.Content, out var parent) && parent != null)
            {
                contentNode = parent;

                if (contentNode == null)
                {
                    return false;
                }

                nfp = config.GetNotFoundPage(contentNode.Key);
                nfpKey = nfp?.Explicit404 ?? nfp?.Inherited404?.Explicit404 ?? Guid.Empty;
                content = umbracoContext.UmbracoContext.Content.GetById(nfpKey);
            }            

            request.SetResponseStatus(404);
            request.SetPublishedContent(content);
            return true;
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
}