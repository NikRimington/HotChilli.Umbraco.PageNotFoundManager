using HC.PageNotFoundManager.Models;
using System;
using System.Threading.Tasks;

namespace HC.PageNotFoundManager.Config;

public interface IPageNotFoundService
{
    PageNotFoundDetails? GetNotFoundPage(int nodeId);

    PageNotFoundDetails? GetNotFoundPage(Guid nodeKey);

    void RefreshCache();

    Task<PageNotFoundDetails> SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache);

    Task<PageNotFoundDetails> SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache);
}