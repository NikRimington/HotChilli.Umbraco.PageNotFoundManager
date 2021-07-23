using HC.PageNotFoundManager.Core.Backoffice;
using HC.PageNotFoundManager.Core.Config;
using HC.PageNotFoundManager.Core.ContentFinders;
using System.Linq;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Core.Startup
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder UsePageNotFoundManager(this IUmbracoBuilder builder)
        {

            if (builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IPageNotFoundConfig)) != null)
                return builder;

            builder.Services.AddUnique<IPageNotFoundConfig, PageNotFoundConfig>();
            builder.SetContentLastChanceFinder<PageNotFoundFinder>();
            builder.BackOfficeAssets()
                .Append<PageNotFoundDialogControllerFile>()
                .Append<PageNotFoundResourceFile>();

            builder.AddNotificationHandler<MenuRenderingNotification,MenuRenderingNotificationHandler>();
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoStartingNotificationHandler>();

            return builder;
        }
    }
}
