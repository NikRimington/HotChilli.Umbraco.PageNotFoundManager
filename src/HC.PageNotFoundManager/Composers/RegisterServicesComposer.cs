using HC.PageNotFoundManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace HC.PageNotFoundManager.Composers
{
    public class RegisterServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IDatabaseService, DatabaseService>();
            builder.Services.AddTransient<ICacheService, CacheService>();
        }
    }
}
