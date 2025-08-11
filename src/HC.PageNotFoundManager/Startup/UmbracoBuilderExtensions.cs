using HC.PageNotFoundManager.Config;
using HC.PageNotFoundManager.ContentFinders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using HC.PageNotFoundManager.Models;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Startup;

public static class UmbracoBuilderExtensions
{
    public static IUmbracoBuilder UsePageNotFoundManager(this IUmbracoBuilder builder)
    {
        if (builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IPageNotFoundService)) != null)
        {
            return builder;
        }

        builder.Services.Configure<PageNotFoundManagerSettings>(builder.Config.GetSection(PageNotFoundManagerSettings.Key));
        builder.Services.ConfigureOptions<Backoffice.Swagger.HCSSwaggerGenOptions>();
        builder.Services.AddUnique<IPageNotFoundService, PageNotFoundConfigService>();
        builder.SetContentLastChanceFinder<PageNotFoundFinder>();
        builder            
            .AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, UmbracoStartingNotificationHandler>();

        return builder;
    }
}