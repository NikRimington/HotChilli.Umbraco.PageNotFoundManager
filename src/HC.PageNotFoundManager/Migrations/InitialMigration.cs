using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace HC.PageNotFoundManager.Migrations;

public class InitialMigration : AsyncMigrationBase
{
    public const string MigrationName = "page-not-found-manager-migration-initial";

    private readonly ILogger<InitialMigration> logger;

    public InitialMigration(IMigrationContext context, ILogger<InitialMigration> logger)
        : base(context)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override Task MigrateAsync()
    {
        logger.LogDebug("Starting migration - {MigrationName}", MigrationName);

        if (!TableExists(PageNotFoundInitialMigrationModel.TableName))
        {
            Create.Table<PageNotFoundInitialMigrationModel>().Do();
        }

        return Task.CompletedTask;
    }
}

public class MigrateV8DataMigration : AsyncMigrationBase
{
    public const string MigrationName = "page-not-found-manager-migration-legacy-data";

    private readonly ILogger<MigrateV8DataMigration> logger;

    public MigrateV8DataMigration(IMigrationContext context, ILogger<MigrateV8DataMigration> logger)
        : base(context)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override Task MigrateAsync()
    {
        logger.LogDebug("Starting migration - {MigrationName}", MigrationName);

        if (TableExists("pageNotFoundConfig"))
        {
            MigrateLegacyData();
        }

        return Task.CompletedTask;
    }

    private void MigrateLegacyData()
    {
        var sql = Sql().Select("unP.uniqueId as ParentId", "unF.uniqueId as NotFoundPageId")
            .From("pageNotFoundConfig as org").LeftJoin("umbracoNode as unP").On("org.ParentId = unP.id")
            .LeftJoin("umbracoNode as unF").On("org.NotFoundPageId = unF.id");

        var toMigrate = Database.Fetch<PageNotFoundInitialMigrationModel>(sql);
        Database.InsertBatch(toMigrate);
    }
}