using System;

namespace HC.PageNotFoundManager.Config;

public interface IPageNotFoundConfig
{
    int GetNotFoundPage(int parentId);

    int GetNotFoundPage(Guid parentKey);

    void RefreshCache();

    void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache);

    void SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache);
}