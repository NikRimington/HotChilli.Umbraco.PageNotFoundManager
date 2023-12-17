using Umbraco.Cms.Infrastructure.Migrations;

namespace HC.PageNotFoundManager.Migrations
{
    public class PageNotFoundMigrationPlan : MigrationPlan
    {
        public PageNotFoundMigrationPlan()
            : base("PageNotFoundManager")
        {
            DefinePlan();
        }

        /// <inheritdoc />
        public override string InitialState => "{pagenotfound-init-state}";

        /// <summary>
        ///     Defines the plan.
        /// </summary>
        protected void DefinePlan()
        {
            From("{pagenotfound-init-state}").To<InitialMigration>("{pagenotfound-init-complete}")
                .To<MigrateV8DataMigration>("{pagenotfound-legacy-data-complete}");
        }
    }
}