using HC.PageNotFoundManager.Config;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace HC.PageNotFoundManager.Backoffice
{
    public class ContentDeletingNotificationHandler : INotificationHandler<ContentDeletingNotification>
    {
        private readonly IPageNotFoundConfig config;

        public ContentDeletingNotificationHandler(IPageNotFoundConfig config) =>
            this.config = config;

        public void Handle(ContentDeletingNotification notification)
        {
            foreach (var content in notification.DeletedEntities)
            {
                if (!config.IsUsedAsNotFoundPage(content.Key))
                    continue;

                notification.CancelOperation(new EventMessage(
                    "Page Not Found Manager",
                    $"'{content.Name}' is configured as a 404 page and cannot be deleted. Remove the 404 page configuration first.",
                    EventMessageType.Error));
                return;
            }
        }
    }
}
