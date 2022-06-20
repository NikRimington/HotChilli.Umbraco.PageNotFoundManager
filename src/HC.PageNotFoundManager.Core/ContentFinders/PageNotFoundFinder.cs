using HC.PageNotFoundManager.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace HC.PageNotFoundManager.Core.ContentFinders
{
    public class PageNotFoundFinder : IContentLastChanceFinder
    {
        private readonly IDomainService _domainService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IPageNotFoundConfig _config;

        public PageNotFoundFinder(IDomainService domainService, IUmbracoContextFactory umbracoContextFactory, IPageNotFoundConfig config)
        {
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            _umbracoContextFactory = umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        public async Task<bool> TryFindContent(IPublishedRequestBuilder request)
        {

            var uri = request.AbsolutePathDecoded;
            // a route is "/path/to/page" when there is no domain, and "123/path/to/page" when there is a domain, and then 123 is the ID of the node which is the root of the domain
            //get domain name from Uri
            // find umbraco home node for uri's domain, and get the id of the node it is set on

            var domains = _domainService.GetAll(true)?.ToList() ?? _domainService.GetAll(true).ToList();
            var domainRoutePrefixId = String.Empty;
            if (domains.Any())
            {
                //A domain can be defined with or without http(s) so we neet to check for both cases.
                IDomain domain = null;
                foreach (IDomain currentDomain in domains)
                {
                    if (IsCurrentDomainIsAbsoluteAndCurrentRequestStartsWithDomain(request, currentDomain) ||
                        CurrentRequestStartsWithDomainAndDomainDoesntStartWithHttp(request, currentDomain))
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
            using (var umbracoContext = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var closestContent = umbracoContext.UmbracoContext.Content.GetByRoute(domainRoutePrefixId + uri.ToString(), false, culture: request?.Culture);
                while (closestContent == null)
                {
                    uri = uri.Remove(uri.Length - 1, 1);
                    closestContent = umbracoContext.UmbracoContext.Content.GetByRoute(domainRoutePrefixId + uri.ToString(), false, culture: request?.Culture);
                }
                var nfp = _config.GetNotFoundPage(closestContent.Id);

                while (nfp == 0)
                {
                    closestContent = closestContent.Parent;

                    if (closestContent == null) return false;

                    nfp = _config.GetNotFoundPage(closestContent.Id);
                }

                var content = umbracoContext.UmbracoContext.Content.GetById(nfp);
                if (content == null) return false;

                request.SetResponseStatus(404);
                request.SetPublishedContent(content);
                return true;
            }

        }

        private static bool CurrentRequestStartsWithDomainAndDomainDoesntStartWithHttp(IPublishedRequestBuilder request, IDomain currentDomain)
        {
            return (request.Uri.Authority.ToLower() + request.Uri.AbsolutePath.ToLower()).StartsWith(currentDomain.DomainName.ToLower());
        }

        private static bool IsCurrentDomainIsAbsoluteAndCurrentRequestStartsWithDomain(IPublishedRequestBuilder request, IDomain currentDomain)
        {
            return currentDomain.DomainName.ToLower().StartsWith("http") && request.Uri.AbsoluteUri.ToLower().StartsWith(currentDomain.DomainName.ToLower());
        }
    }

}
