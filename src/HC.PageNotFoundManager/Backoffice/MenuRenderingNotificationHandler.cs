using System;
using System.Collections.Generic;
using System.Linq;
using HC.PageNotFoundManager.Config;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Backoffice
{
    public class MenuRenderingNotificationHandler : INotificationHandler<MenuRenderingNotification>
    {

        private readonly IBackOfficeSecurityAccessor backOfficeSecurity;

        private readonly List<string> userGroupNames;

        public MenuRenderingNotificationHandler(
            IOptions<PageNotFound> settings,
            IBackOfficeSecurityAccessor backOfficeSecurity)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            userGroupNames = new List<string> { Umbraco.Cms.Core.Constants.Security.AdminGroupAlias };
            if (settings.Value.UserGroups != null && settings.Value.UserGroups.Length > 0)
            {
                userGroupNames.AddRange(settings.Value.UserGroups);
            }

            this.backOfficeSecurity = backOfficeSecurity ?? throw new ArgumentNullException(nameof(backOfficeSecurity));
        }

        public void Handle(MenuRenderingNotification notification)
        {
            if (notification.TreeAlias != Umbraco.Cms.Core.Constants.Trees.Content
                || (int.TryParse(notification.NodeId, out int nodeId) && nodeId <= 0))
            {
                return;
            }

            if (backOfficeSecurity.BackOfficeSecurity?.CurrentUser?.Groups == null
                || backOfficeSecurity.BackOfficeSecurity.CurrentUser.Groups.All(
                    g => !userGroupNames.InvariantContains(g.Alias)))
            {
                return;
            }

            var menuItem = new MenuItem(Constants.Constants.MenuAlias, Constants.Constants.MenuLabel)
                           {
                               Icon = "directions color-blue", SeparatorBefore = true
                           };

            menuItem.LaunchDialogView(
                "/App_Plugins/HC.PageNotFound/Backoffice/Dialogs/dialog.html",
                Constants.Constants.MenuLabel);

            notification.Menu.Items.Add(menuItem);
        }
    }
}