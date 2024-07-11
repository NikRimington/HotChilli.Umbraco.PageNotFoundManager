using System.Linq;
using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.ContentFinders;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Startup;

public static class UmbracoBuilderExtensions
{
    public static IUmbracoBuilder UsePageNotFoundManager(this IUmbracoBuilder builder)
    {
        if (builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IPageNotFoundConfig)) != null)
        {
            return builder;
        }

        builder.Services.ConfigureOptions<Backoffice.Swagger.HCSSwaggerGenOptions>();
        builder.Services.AddUnique<IPageNotFoundConfig, PageNotFoundConfig>();
        builder.SetContentLastChanceFinder<PageNotFoundFinder>();
        builder
            .AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoStartingNotificationHandler>();

        return builder;
    }
}