using HC.PageNotFoundManager.Notifications;
using HC.PageNotFoundManager.uSync.Handlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace HC.PageNotFoundManager.uSync.Composers
{
    public class RegisterNotifications : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<OnConfigurationSavedNotification, PageNotFoundManagerHandler>();
        }
    }
}
