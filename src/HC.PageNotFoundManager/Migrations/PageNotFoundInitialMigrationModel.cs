using System;
using System.Runtime.Serialization;
using NPoco;

namespace HC.PageNotFoundManager.Migrations
{
    [TableName(TableName)]
    [PrimaryKey("ParentId", AutoIncrement = false)]
    public sealed class PageNotFoundInitialMigrationModel
    {
        [IgnoreDataMember]
        public const string TableName = "PageNotFoundManagerConfig";

        [Column("NotFoundPageId")] public Guid NotFoundPageId { get; set; }

        [Column("ParentId")] public Guid ParentId { get; set; }
    }
}