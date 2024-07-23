using System;

namespace HC.PageNotFoundManager.Models;

public class PageNotFoundRequest
{

    public Guid? NotFoundPageId { get; set; }

    public Guid ParentId { get; set; }
}