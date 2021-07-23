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

    public class PageNotFoundMigrationPlan : MigrationPlan
    {

        public PageNotFoundMigrationPlan()
            : base("PageNotFoundManager") => DefinePlan();

        /// <inheritdoc/>
        public override string InitialState => "{pagenotfound-init-state}";

        /// <summary>
        /// Defines the plan.
        /// </summary>
        protected void DefinePlan()
        {
            From("{pagenotfound-init-state}")
                .To<InitialMigration>("{pagenotfound-init-complete}");
        }
    }

}
