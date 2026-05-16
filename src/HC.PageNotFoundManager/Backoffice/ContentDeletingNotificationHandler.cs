using System.Threading;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Config;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace HC.PageNotFoundManager.Backoffice;

public class ContentDeletingNotificationHandler : INotificationAsyncHandler<ContentDeletingNotification>
{
    private readonly IPageNotFoundService service;

    public ContentDeletingNotificationHandler(IPageNotFoundService service) =>
        this.service = service;

    public Task HandleAsync(ContentDeletingNotification notification, CancellationToken cancellationToken)
    {
        foreach (var content in notification.DeletedEntities)
        {
            if (!service.IsUsedAsNotFoundPage(content.Key))
                continue;

            notification.CancelOperation(new EventMessage(
                "Page Not Found Manager",
                $"'{content.Name}' is configured as a 404 page and cannot be deleted. Remove the 404 page configuration first.",
                EventMessageType.Error));
            break;
        }

        return Task.CompletedTask;
    }
}
