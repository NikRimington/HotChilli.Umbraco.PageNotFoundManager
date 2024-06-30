using System.Collections.Generic;
using Umbraco.Cms.Core.Notifications;

namespace HC.PageNotFoundManager.Notifications
{
    public class OnConfigurationSavedNotification : INotification
    {
        public IEnumerable<Models.PageNotFound>? Configuration { get; }

        public OnConfigurationSavedNotification(IEnumerable<Models.PageNotFound>? configuration)
        {
            Configuration = configuration;
        }
    }
}
