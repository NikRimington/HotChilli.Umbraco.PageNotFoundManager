using HC.PageNotFoundManager.Core.Config;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Core.Backoffice
{
    public class MenuRenderingNotificationHandler : INotificationHandler<MenuRenderingNotification>
    {
        private readonly List<string> UserGroupNames;
        private readonly IBackOfficeSecurityAccessor backOfficeSecurity;

        public MenuRenderingNotificationHandler(IOptions<PageNotFound> settings, IBackOfficeSecurityAccessor backOfficeSecurity)
        {
            if (settings is null) throw new System.ArgumentNullException(nameof(settings));

            UserGroupNames = new List<string> { Umbraco.Cms.Core.Constants.Security.AdminGroupAlias };
            if (settings.Value.UserGroups != null && settings.Value.UserGroups.Length > 0)
                UserGroupNames.AddRange(settings.Value.UserGroups);
            this.backOfficeSecurity = backOfficeSecurity ?? throw new System.ArgumentNullException(nameof(backOfficeSecurity));
        }

        public void Handle(MenuRenderingNotification notification)
        {
            if (notification.TreeAlias != Umbraco.Cms.Core.Constants.Trees.Content
                || int.TryParse(notification.NodeId, out var nodeId) && nodeId <= 0) return;

            if (backOfficeSecurity.BackOfficeSecurity.CurrentUser == null) return;

            if (backOfficeSecurity.BackOfficeSecurity.CurrentUser.Groups == null ||
                backOfficeSecurity.BackOfficeSecurity.CurrentUser.Groups.All(g => !UserGroupNames.InvariantContains(g.Alias))) return;

            var menuItem = new MenuItem(Constants.MenuAlias, Constants.MenuLabel)
            {
                Icon = "directions color-blue",
                SeparatorBefore = true
            };

            menuItem.LaunchDialogView("/App_Plugins/HC.PageNotFound/Backoffice/Dialogs/dialog.html", Constants.MenuLabel);

            notification.Menu.Items.Add(menuItem);

        }
    }
}
