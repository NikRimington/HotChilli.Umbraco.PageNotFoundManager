using System;
using System.Collections.Generic;

namespace HC.PageNotFoundManager.Services
{
    public interface IDatabaseService
    {
        IEnumerable<Models.PageNotFound> LoadFromDb();
        void InsertToDb(Guid parentKey, Guid pageNotFoundKey);
    }
}
