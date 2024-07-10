using System.Xml.Linq;
using HC.PageNotFoundManager.Models;
using HC.PageNotFoundManager.Services;
using Microsoft.Extensions.Logging;
using uSync.Core;
using uSync.Core.Models;
using uSync.Core.Serialization;

namespace HC.PageNotFoundManager.uSync.Serializers
{
    [SyncSerializer("6ca9a472-98f3-42f1-82ad-d1a8cbde5fc5", Consts.Configuration.SerializerName,
        Consts.Configuration.ItemType)]
    public class PageNotFoundManagerSerializer : SyncSerializerRoot<IEnumerable<PageNotFound>>,
        ISyncSerializer<IEnumerable<PageNotFound>>
    {
        private readonly IDatabaseService databaseService;

        private readonly ICacheService cacheService;

        public PageNotFoundManagerSerializer(ILogger<SyncSerializerRoot<IEnumerable<PageNotFound>>> logger, IDatabaseService databaseService, ICacheService cacheService) : base(logger)
        {
            this.databaseService = databaseService;
            this.cacheService = cacheService;
        }

        protected override SyncAttempt<XElement> SerializeCore(IEnumerable<PageNotFound> item,
            SyncSerializerOptions options)
        {
            var node = new XElement(ItemType,
                new XAttribute("Alias", ItemAlias(item)),
                new XAttribute("Key", ItemKey(item)));

            node.Add(item.Select(item =>
                new XElement("PageNotFound",
                    new XElement("ParentId", item.ParentId),
                    new XElement("NotFoundPageId", item.NotFoundPageId)
                )
            ));

            return SyncAttempt<XElement>.Succeed(Consts.Configuration.ItemType, node, typeof(IEnumerable<PageNotFound>), ChangeType.Export);
        }

        protected override SyncAttempt<IEnumerable<PageNotFound>> DeserializeCore(XElement node,
            SyncSerializerOptions options)
        {
            var item = new List<PageNotFound>();

            foreach (var element in node.Elements("PageNotFound"))
            {
                item.Add(new PageNotFound
                {
                    ParentId = element.Element("ParentId").ValueOrDefault(Guid.Empty),
                    NotFoundPageId = element.Element("NotFoundPageId").ValueOrDefault(Guid.Empty)
                });
            }

            return SyncAttempt<IEnumerable<PageNotFound>>.Succeed("Configuration", item, ChangeType.Import, Array.Empty<uSyncChange>());
        }

        public override IEnumerable<PageNotFound> FindItem(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PageNotFound> FindItem(Guid key) => databaseService.LoadFromDb() ?? new List<PageNotFound>();

        public override IEnumerable<PageNotFound> FindItem(string alias)
        {
            throw new NotImplementedException();
        }

        public override void SaveItem(IEnumerable<PageNotFound> items)
        {
            foreach (var item in items)
            {
                databaseService.InsertToDb(item.ParentId, item.NotFoundPageId);
            }

            cacheService.RefreshCache();
        }

        public override void DeleteItem(IEnumerable<PageNotFound> item)
        {
            throw new NotImplementedException();
        }

        public override string ItemAlias(IEnumerable<PageNotFound> item) => "pageNotFoundManager";

        public override Guid ItemKey(IEnumerable<PageNotFound> item) => Guid.Parse("cc346b54-54f8-46c0-9290-1214e9639a58");
    }
}
