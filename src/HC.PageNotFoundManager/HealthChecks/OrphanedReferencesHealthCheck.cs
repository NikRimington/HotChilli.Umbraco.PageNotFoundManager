using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Models.DatabaseModels;
using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.HealthChecks;

[HealthCheck(
    "3f8a7b2c-4d5e-6f7a-8b9c-0d1e2f3a4b5c",
    "Page Not Found Manager - Orphaned References",
    Description = "Checks that all configured 404 page references point to content nodes that still exist.",
    Group = "Page Not Found Manager")]
public class OrphanedReferencesHealthCheck : HealthCheck
{
    private const string RemoveOrphansAction = "removeOrphans";

    private readonly IScopeProvider scopeProvider;
    private readonly IContentService contentService;

    public OrphanedReferencesHealthCheck(IScopeProvider scopeProvider, IContentService contentService)
    {
        this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        this.contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
    }

    public override Task<IEnumerable<HealthCheckStatus>> GetStatusAsync() =>
        Task.FromResult<IEnumerable<HealthCheckStatus>>([CheckForOrphans()]);

    public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
    {
        if (action.Alias != RemoveOrphansAction)
            return new HealthCheckStatus("Unknown action.") { ResultType = StatusResultType.Error };

        var orphans = GetOrphanedRecords();
        var removed = 0;

        using var scope = scopeProvider.CreateScope();
        foreach (var record in orphans)
        {
            scope.Database.Delete(record);
            removed++;
        }
        scope.Complete();

        return new HealthCheckStatus($"Removed {removed} orphaned reference(s).")
        {
            ResultType = StatusResultType.Success
        };
    }

    private HealthCheckStatus CheckForOrphans()
    {
        var orphans = GetOrphanedRecords().ToList();

        if (orphans.Count == 0)
        {
            return new HealthCheckStatus("All configured 404 page references are valid.")
            {
                ResultType = StatusResultType.Success
            };
        }

        var details = string.Join(", ", orphans.Select(o => $"ParentId={o.ParentId} → NotFoundPageId={o.NotFoundPageId}"));

        return new HealthCheckStatus($"{orphans.Count} orphaned reference(s) found: {details}")
        {
            ResultType = StatusResultType.Warning,
            Actions =
            [
                new HealthCheckAction(RemoveOrphansAction, Id)
                {
                    Name = "Remove Orphaned References",
                    Description = "Deletes all orphaned 404 page configuration records."
                }
            ]
        };
    }

    private IEnumerable<PageNotFound> GetOrphanedRecords()
    {
        List<PageNotFound> records;

        using (var scope = scopeProvider.CreateScope(autoComplete: true))
        {
            var sql = scope.SqlContext.Sql().Select("*").From<PageNotFound>();
            records = scope.Database.Fetch<PageNotFound>(sql);
        }

        return records.Where(r =>
            contentService.GetById(r.ParentId) is null ||
            contentService.GetById(r.NotFoundPageId) is null);
    }
}
