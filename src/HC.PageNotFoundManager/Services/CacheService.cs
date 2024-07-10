using Umbraco.Cms.Core.Cache;

namespace HC.PageNotFoundManager.Services
{
    internal class CacheService : ICacheService
    {
        private const string CacheKey = "PageNotFoundConfig";

        private readonly IAppPolicyCache appPolicyCache;

        private readonly IDatabaseService databaseService;

        public CacheService(IAppPolicyCache appPolicyCache, IDatabaseService databaseService)
        {
            this.appPolicyCache = appPolicyCache;
            this.databaseService = databaseService;
        }

        public void RefreshCache()
        {
            appPolicyCache.ClearByKey(CacheKey);
            appPolicyCache.Insert(CacheKey, databaseService.LoadFromDb);
        }
    }
}
