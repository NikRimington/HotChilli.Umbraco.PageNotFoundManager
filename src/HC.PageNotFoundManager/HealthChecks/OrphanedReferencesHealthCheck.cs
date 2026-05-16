using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HC.PageNotFoundManager.Models;
using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.HealthChecks
{
    [HealthCheck(
        "3f8a7b2c-4d5e-6f7a-8b9c-0d1e2f3a4b5c",
        "Page Not Found Manager - Orphaned References",
        Description = "Checks that all configured 404 page references point to content nodes that still exist.",
        Group = "Page Not Found Manager")]
    public class OrphanedReferencesHealthCheck : HealthCheck
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IContentService _contentService;

        public OrphanedReferencesHealthCheck(IScopeProvider scopeProvider, IContentService contentService)
        {
            _scopeProvider = scopeProvider;
            _contentService = contentService;
        }

        public override Task<IEnumerable<HealthCheckStatus>> GetStatus()
        {
            var orphans = GetOrphanedRecords();

            if (orphans.Count == 0)
            {
                return Task.FromResult<IEnumerable<HealthCheckStatus>>(new[]
                {
                    new HealthCheckStatus("All configured 404 page references are valid.")
                    {
                        ResultType = StatusResultType.Success
                    }
                });
            }

            var descriptions = orphans.Select(DescribeOrphan);
            var detail = string.Join(", ", descriptions);

            return Task.FromResult<IEnumerable<HealthCheckStatus>>(new[]
            {
                new HealthCheckStatus(
                    $"{orphans.Count} orphaned reference(s) found in PageNotFoundManagerConfig: {detail}")
                {
                    ResultType = StatusResultType.Warning,
                    Actions = new List<HealthCheckAction>
                    {
                        new HealthCheckAction("removeOrphans", Id)
                        {
                            Name = "Remove Orphaned References",
                            Description = $"Delete {orphans.Count} orphaned record(s) from the database table."
                        }
                    }
                }
            });
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            if (action.Alias != "removeOrphans")
                throw new InvalidOperationException($"Unknown action alias: {action.Alias}");

            var orphans = GetOrphanedRecords();

            using (var scope = _scopeProvider.CreateScope())
            {
                foreach (var orphan in orphans)
                {
                    scope.Database.Delete(orphan);
                }

                scope.Complete();
            }

            return new HealthCheckStatus($"Removed {orphans.Count} orphaned reference(s) successfully.")
            {
                ResultType = StatusResultType.Success
            };
        }

        private List<PageNotFound> GetOrphanedRecords()
        {
            List<PageNotFound> allRecords;

            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.SqlContext.Sql().Select("*").From<PageNotFound>();
                allRecords = scope.Database.Fetch<PageNotFound>(sql);
            }

            return allRecords
                .Where(r =>
                    _contentService.GetById(r.ParentId) == null ||
                    _contentService.GetById(r.NotFoundPageId) == null)
                .ToList();
        }

        private static string DescribeOrphan(PageNotFound record) =>
            $"ParentId={record.ParentId} → NotFoundPageId={record.NotFoundPageId}";
    }
}
