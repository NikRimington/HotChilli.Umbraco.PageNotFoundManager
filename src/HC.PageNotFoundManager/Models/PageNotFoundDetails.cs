using System;

namespace HC.PageNotFoundManager.Models;

public class PageNotFoundDetails
{
    public Guid PageId { get; set;}
    public Guid? Explicit404 { get; set; }
    public PageNotFoundDetails? Inherited404 { get; set; }

    public bool Has404()
    {
        return (Explicit404 != null && Explicit404.Value != Guid.Empty)
            || Inherited404 != null;
    }

}