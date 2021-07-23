using NPoco;
using System;
using System.Runtime.Serialization;

namespace HC.PageNotFoundManager.Core.Models
{
    [TableName(TableName)]

    [PrimaryKey("ParentId", AutoIncrement = false)]
    public class PageNotFound
    {

        [IgnoreDataMember]
        public const string TableName = "PageNotFoundManagerConfig";

        [Column("ParentId")]
        public Guid ParentId { get; set; }

        [Column("NotFoundPageId")]
        public Guid NotFoundPageId { get; set; }

    }
}
