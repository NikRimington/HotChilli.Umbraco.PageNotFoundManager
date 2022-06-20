using Microsoft.Extensions.Logging;
using System;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace HC.PageNotFoundManager.Core.Startup
{
    public class UmbracoStartingNotificationHandler : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly ILogger<UmbracoStartingNotificationHandler> _logger;
        private readonly IRuntimeState _runtimeState;
        private readonly IScopeProvider _scopeProvider;
        private readonly IKeyValueService _keyValueService;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;

        public UmbracoStartingNotificationHandler(ILogger<UmbracoStartingNotificationHandler> logger,
            IRuntimeState runtimeState, IScopeProvider scopeProvider, IKeyValueService keyValueService, IMigrationPlanExecutor migrationPlanExecutor)
        {
            
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
            this._scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            this._keyValueService = keyValueService ?? throw new ArgumentNullException(nameof(keyValueService));
            this._migrationPlanExecutor = migrationPlanExecutor ?? throw new ArgumentNullException(nameof(migrationPlanExecutor));
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (_runtimeState.Level < RuntimeLevel.Upgrade)
            {
                _logger.LogInformation("Umbraco Runtime is not Run/Upgrade mode, so a database connection is unlikely to be available for migrations");
                return;
            }

            ApplyMigration();
        }

        private void ApplyMigration()
        {
            var upgrader = new Upgrader(new Migrations.PageNotFoundMigrationPlan());
            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }
    }
}
