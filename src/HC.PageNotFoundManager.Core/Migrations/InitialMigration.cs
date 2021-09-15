using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace HC.PageNotFoundManager.Core.Migrations
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
            logger.LogDebug("Starting migration - {migrationName}", MigrationName);

            if (!TableExists(PageNotFoundInitialMigrationModel.TableName))
                Create.Table<PageNotFoundInitialMigrationModel>().Do();
        }
    }

    public class MigrateV8DataMigration : MigrationBase
    {
        public const string MigrationName = "page-not-found-manager-migration-legacy-data";
        private readonly ILogger<MigrateV8DataMigration> logger;

        public MigrateV8DataMigration(IMigrationContext context, ILogger<MigrateV8DataMigration> logger) : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        protected override void Migrate()
        {
            logger.LogDebug("Starting migration - {migrationName}", MigrationName);

            if (TableExists("pageNotFoundConfig"))
                MigrateLegacyData();
        }

        private void MigrateLegacyData()
        {
            var sql = this.Sql().Select("unP.uniqueId as ParentId", "unF.uniqueId as NotFoundPageId").From("pageNotFoundConfig as org")
                .LeftJoin("umbracoNode as unP").On("org.ParentId = unP.id")
                .LeftJoin("umbracoNode as unF").On("org.NotFoundPageId = unF.id");

            var toMigrate = Database.Fetch<PageNotFoundInitialMigrationModel>(sql);
            Database.InsertBatch(toMigrate);                
        }
    }

}
