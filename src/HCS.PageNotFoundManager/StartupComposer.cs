using HCS.PageNotFoundManager.Core.Config;
using HCS.PageNotFoundManager.Core.ContentFinders;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace HCS.PageNotFoundManager.Core
{
    public class StartupComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<IPageNotFoundConfig, PageNotFoundConfig>();
            builder.SetContentLastChanceFinder<PageNotFoundFinder>();
        }
    }
}
