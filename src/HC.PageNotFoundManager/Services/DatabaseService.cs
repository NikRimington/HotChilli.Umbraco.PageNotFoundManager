using System;
using System.Collections.Generic;
using HC.PageNotFoundManager.Notifications;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Services
{
    internal class DatabaseService : IDatabaseService
    {
        private readonly IScopeProvider scopeProvider;

        public DatabaseService(IScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }

        public IEnumerable<Models.PageNotFound> LoadFromDb()
        {
            using var scope = scopeProvider.CreateScope(autoComplete: true);
            var sql = scope.SqlContext.Sql().Select("*").From<Models.PageNotFound>();
            var pages = scope.Database.Fetch<Models.PageNotFound>(sql);
            scope.Complete();
            return pages;
        }

        public void InsertToDb(Guid parentKey, Guid pageNotFoundKey)
        {
            using var scope = scopeProvider.CreateScope();
            var db = scope.Database;
            var page = db.Query<Models.PageNotFound>().Where(p => p.ParentId == parentKey).FirstOrDefault();
            if (page == null && !Guid.Empty.Equals(pageNotFoundKey))
            {
                // create the page
                db.Insert(new Models.PageNotFound { ParentId = parentKey, NotFoundPageId = pageNotFoundKey });
            }
            else if (page != null)
            {
                if (Guid.Empty.Equals(pageNotFoundKey))
                {
                    db.Delete(page);
                }
                else
                {
                    // update the existing page
                    page.NotFoundPageId = pageNotFoundKey;
                    db.Update(Models.PageNotFound.TableName, "ParentId", page);
                }
            }

            scope.Notifications.Publish(new OnConfigurationSavedNotification(LoadFromDb()));

            scope.Complete();
        }
    }
}
