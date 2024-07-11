using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace HC.PageNotFoundManager.Startup;

public class StartupComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.UsePageNotFoundManager();
    }
}