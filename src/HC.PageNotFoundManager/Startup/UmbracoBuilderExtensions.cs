using System.Linq;
using HC.PageNotFoundManager.Backoffice;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.ContentFinders;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Startup
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder UsePageNotFoundManager(this IUmbracoBuilder builder)
        {
            if (builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IPageNotFoundConfig)) != null)
            {
                return builder;
            }

            builder.Services.AddUnique<IPageNotFoundConfig, PageNotFoundConfig>();
            builder.SetContentLastChanceFinder<PageNotFoundFinder>();

            builder.ManifestFilters().Append<PageNotFoundManifestFilter>();

            builder.AddNotificationHandler<MenuRenderingNotification, MenuRenderingNotificationHandler>();
            builder
                .AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoStartingNotificationHandler>();

            return builder;
        }
    }
}