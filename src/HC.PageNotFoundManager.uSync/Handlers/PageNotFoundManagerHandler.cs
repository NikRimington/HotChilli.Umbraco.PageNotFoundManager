using HC.PageNotFoundManager.Notifications;
using HC.PageNotFoundManager.Services;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using uSync.BackOffice;
using uSync.BackOffice.Configuration;
using uSync.BackOffice.Services;
using uSync.BackOffice.SyncHandlers;
using uSync.Core;
using PageNotFound = HC.PageNotFoundManager.Models.PageNotFound;

namespace HC.PageNotFoundManager.uSync.Handlers
{
    [SyncHandler("pageNotFoundManagerHander", Consts.Configuration.HandlerName, Consts.Configuration.SerializerFolder, 1,
        Icon = "icon-settings usync-addon-icon", EntityType = Consts.Configuration.EntityType)]
    public class PageNotFoundManagerHandler : SyncHandlerRoot<IEnumerable<PageNotFound>, IEnumerable<PageNotFound>>, ISyncHandler,
        INotificationHandler<OnConfigurationSavedNotification>
    {
        public override string Group => Consts.Group;

        private readonly IDatabaseService databaseService;

        public PageNotFoundManagerHandler(ILogger<SyncHandlerRoot<IEnumerable<PageNotFound>, IEnumerable<PageNotFound>>> logger, AppCaches appCaches, IShortStringHelper shortStringHelper, SyncFileService syncFileService, uSyncEventService mutexService, uSyncConfigService uSyncConfig, ISyncItemFactory itemFactory, IDatabaseService databaseService) : base(logger, appCaches, shortStringHelper, syncFileService, mutexService, uSyncConfig, itemFactory)
        {
            this.databaseService = databaseService;

            itemContainerType = UmbracoObjectTypes.Unknown;
        }

        public override IEnumerable<uSyncAction> ExportAll(IEnumerable<PageNotFound> items, string folder, HandlerSettings config,
            SyncUpdateCallback callback)
        {
            var item = databaseService.LoadFromDb();

            var actions = new List<uSyncAction>();
            if (item != null)
            {
                actions.AddRange(Export(item, Path.Combine(rootFolder, DefaultFolder), DefaultConfig));
            }

            return actions;
        }

        protected override IEnumerable<IEnumerable<PageNotFound>> GetChildItems(IEnumerable<PageNotFound> parent)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IEnumerable<PageNotFound>> GetFolders(IEnumerable<PageNotFound> parent)
        {
            throw new NotImplementedException();
        }

        public void Handle(OnConfigurationSavedNotification notification)
        {
            if (!ShouldProcess()) return;

            try
            {
                if (notification.Configuration != null)
                {
                    var attempts = Export(notification.Configuration, Path.Combine(rootFolder, DefaultFolder), DefaultConfig);
                    foreach (var attempt in attempts.Where(x => x.Success))
                    {
                        CleanUp(notification.Configuration, attempt.FileName, Path.Combine(rootFolder, DefaultFolder));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "uSync Save error");
            }
        }

        protected override IEnumerable<uSyncAction> DeleteMissingItems(IEnumerable<PageNotFound> items, IEnumerable<Guid> keysToKeep, bool reportOnly)
            => Enumerable.Empty<uSyncAction>();

        protected override IEnumerable<PageNotFound> GetFromService(IEnumerable<PageNotFound> items)
            => databaseService.LoadFromDb() ?? new List<PageNotFound>();

        protected override string GetItemFileName(IEnumerable<PageNotFound> items)
            => Consts.Configuration.FileName;

        protected override string GetItemName(IEnumerable<PageNotFound> item)
        {
            throw new NotImplementedException();
        }

        private bool ShouldProcess()
        {
            if (_mutexService.IsPaused) return false;
            if (!DefaultConfig.Enabled) return false;
            return true;
        }
    }
}
