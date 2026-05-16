using HC.PageNotFoundManager.Config;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace HC.PageNotFoundManager.Backoffice
{
    public class ContentMovingToRecycleBinNotificationHandler : INotificationHandler<ContentMovingToRecycleBinNotification>
    {
        private readonly IPageNotFoundConfig config;

        public ContentMovingToRecycleBinNotificationHandler(IPageNotFoundConfig config) =>
            this.config = config;

        public void Handle(ContentMovingToRecycleBinNotification notification)
        {
            foreach (var info in notification.MoveInfoCollection)
            {
                if (!config.IsUsedAsNotFoundPage(info.Entity.Key))
                    continue;

                notification.CancelOperation(new EventMessage(
                    "Page Not Found Manager",
                    $"'{info.Entity.Name}' is configured as a 404 page and cannot be moved to the recycle bin. Remove the 404 page configuration first.",
                    EventMessageType.Error));
                return;
            }
        }
    }
}
