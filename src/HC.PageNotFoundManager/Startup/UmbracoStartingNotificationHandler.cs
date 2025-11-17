
using System;
using System.Threading;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Migrations;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace HC.PageNotFoundManager.Startup;

public class UmbracoStartingNotificationHandler : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
    private readonly IKeyValueService keyValueService;

    private readonly ILogger<UmbracoStartingNotificationHandler> logger;

    private readonly IMigrationPlanExecutor migrationPlanExecutor;

    private readonly IRuntimeState runtimeState;

    private readonly IScopeProvider scopeProvider;

    public UmbracoStartingNotificationHandler(
        ILogger<UmbracoStartingNotificationHandler> logger,
        IRuntimeState runtimeState,
        IScopeProvider scopeProvider,
        IKeyValueService keyValueService,
        IMigrationPlanExecutor migrationPlanExecutor)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
        this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        this.keyValueService = keyValueService ?? throw new ArgumentNullException(nameof(keyValueService));
        this.migrationPlanExecutor =
            migrationPlanExecutor ?? throw new ArgumentNullException(nameof(migrationPlanExecutor));
    }

    public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
    {
       if (runtimeState.Level < RuntimeLevel.Upgrade)
        {
            logger.LogInformation(
                "Umbraco Runtime is not Run/Upgrade mode, so a database connection is unlikely to be available for migrations");
            return;
        }

        await ApplyMigration();
    }

    private async Task<ExecutedMigrationPlan> ApplyMigration()
    {
        var upgrader = new Upgrader(new PageNotFoundMigrationPlan());
        return await upgrader.ExecuteAsync(migrationPlanExecutor, scopeProvider, keyValueService);
    }
}