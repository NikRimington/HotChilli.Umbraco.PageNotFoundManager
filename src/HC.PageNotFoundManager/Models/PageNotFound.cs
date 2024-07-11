using System;
using System.Runtime.Serialization;
using NPoco;

namespace HC.PageNotFoundManager.Models;

[TableName(TableName)]
[PrimaryKey("ParentId", AutoIncrement = false)]
public class PageNotFound
{
    [IgnoreDataMember]
    public const string TableName = "PageNotFoundManagerConfig";

    [Column("NotFoundPageId")] public Guid NotFoundPageId { get; set; }

    [Column("ParentId")] public Guid ParentId { get; set; }
}