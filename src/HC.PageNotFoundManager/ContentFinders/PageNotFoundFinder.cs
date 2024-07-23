using System;
using System.Linq;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Config;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace HC.PageNotFoundManager.ContentFinders;

public class PageNotFoundFinder : IContentLastChanceFinder
{
    private readonly IPageNotFoundConfig config;

    private readonly IDomainService domainService;

    private readonly IUmbracoContextFactory umbracoContextFactory;

    public PageNotFoundFinder(
        IDomainService domainService,
        IUmbracoContextFactory umbracoContextFactory,
        IPageNotFoundConfig config)
    {
        this.domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        this.umbracoContextFactory =
            umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
        this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<bool> TryFindContent(IPublishedRequestBuilder request)
    {
        string uri = request.AbsolutePathDecoded;
        // a route is "/path/to/page" when there is no domain, and "123/path/to/page" when there is a domain, and then 123 is the ID of the node which is the root of the domain
        //get domain name from Uri
        // find umbraco home node for uri's domain, and get the id of the node it is set on

        var domains = (await domainService.GetAllAsync(true)).ToList();
        var domainRoutePrefixId = string.Empty;
        if (domains.Count != 0)
        {
            //A domain can be defined with or without http(s) so we neet to check for both cases.
            IDomain domain = null;
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
            var closestContent = umbracoContext.UmbracoContext.Content?.GetByRoute(
                domainRoutePrefixId + uri,
                false,
                request?.Culture);
            while (closestContent == null)
            {
                uri = uri.Remove(uri.Length - 1, 1);
                closestContent = umbracoContext.UmbracoContext.Content?.GetByRoute(
                    domainRoutePrefixId + uri,
                    false,
                    request?.Culture);
            }

            var nfp = config.GetNotFoundPage(closestContent.Key);

            while (nfp == null || nfp == Guid.Empty)
            {
                closestContent = closestContent.Parent;

                if (closestContent == null)
                {
                    return false;
                }

                nfp = config.GetNotFoundPage(closestContent.Key);
            }

            var content = umbracoContext.UmbracoContext.Content.GetById(nfp ?? Guid.Empty);
            if (content == null)
            {
                return false;
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