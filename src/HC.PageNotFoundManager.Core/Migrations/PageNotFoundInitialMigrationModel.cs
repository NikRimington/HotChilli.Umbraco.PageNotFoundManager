using NPoco;
using System;
using System.Runtime.Serialization;

namespace HC.PageNotFoundManager.Core.Migrations
{
    [TableName(TableName)]
    [PrimaryKey("ParentId", AutoIncrement = false)]
    public sealed class PageNotFoundInitialMigrationModel
    {

        [IgnoreDataMember]
        public const string TableName = "PageNotFoundManagerConfig";

        [Column("ParentId")]
        public Guid ParentId { get; set; }

        [Column("NotFoundPageId")]
        public Guid NotFoundPageId { get; set; }

    }

}
