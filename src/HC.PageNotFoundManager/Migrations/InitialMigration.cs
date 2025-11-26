using System;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace HC.PageNotFoundManager.Migrations
{
    public class InitialMigration : MigrationBase
    {
        public const string MigrationName = "page-not-found-manager-migration-initial";

        private readonly ILogger<InitialMigration> logger;

        public InitialMigration(IMigrationContext context, ILogger<InitialMigration> logger)
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Migrate()
        {
            logger.LogDebug("Starting migration - {MigrationName}", MigrationName);

            if (!TableExists(PageNotFoundInitialMigrationModel.TableName))
            {
                Create.Table<PageNotFoundInitialMigrationModel>().Do();
            }
        }
    }
}