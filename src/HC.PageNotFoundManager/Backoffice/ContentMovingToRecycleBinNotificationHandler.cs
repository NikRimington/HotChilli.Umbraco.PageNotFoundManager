using System.Threading;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Config;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace HC.PageNotFoundManager.Backoffice;

public class ContentMovingToRecycleBinNotificationHandler : INotificationAsyncHandler<ContentMovingToRecycleBinNotification>
{
    private readonly IPageNotFoundService service;

    public ContentMovingToRecycleBinNotificationHandler(IPageNotFoundService service) =>
        this.service = service;

    public Task HandleAsync(ContentMovingToRecycleBinNotification notification, CancellationToken cancellationToken)
    {
        foreach (var info in notification.MoveInfoCollection)
        {
            if (!service.IsUsedAsNotFoundPage(info.Entity.Key))
                continue;

            notification.CancelOperation(new EventMessage(
                "Page Not Found Manager",
                $"'{info.Entity.Name}' is configured as a 404 page and cannot be moved to the recycle bin. Remove the 404 page configuration first.",
                EventMessageType.Error));
            break;
        }

        return Task.CompletedTask;
    }
}
