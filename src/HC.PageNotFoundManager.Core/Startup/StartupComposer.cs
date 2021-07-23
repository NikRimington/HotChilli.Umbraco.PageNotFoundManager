using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace HC.PageNotFoundManager.Core.Startup
{    
    public class StartupComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.UsePageNotFoundManager();
        }
    }
}
