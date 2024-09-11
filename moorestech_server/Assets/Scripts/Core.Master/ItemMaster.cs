using System;
using System.Collections.Generic;
using System.Linq;
using Mooresmaster.Loader.ItemsModule;
using Mooresmaster.Model.ItemsModule;
using Newtonsoft.Json.Linq;
using UnitGenerator;

namespace Core.Master
{
    // アイテムId専用の方を定義
    // NOTE このIDは永続化されれることはなく、メモリ上、ネットワーク通信上でのみ使用する値
    [UnitOf(typeof(int))]
    public partial struct ItemId { }
    
    public class ItemMaster
    {
        public readonly Items Items;
        
        private readonly Dictionary<ItemId,ItemElement> _itemElementTableById; 
        private readonly Dictionary<Guid,ItemId> _itemGuidToItemId;
        
        public ItemMaster(JToken itemJToken)
        {
            // GUIDの順番にint型のItemIdを割り当てる
            Items = ItemsLoader.Load(itemJToken);
            
            var sortedItemElements = Items.Data.ToList().OrderBy(x => x.ItemGuid).ToList();
            
            // アイテムID 0は空のアイテムとして予約しているので、1から始める
            _itemElementTableById = new Dictionary<ItemId,ItemElement>();
            _itemGuidToItemId = new Dictionary<Guid,ItemId>();
            for (var i = 1; i < sortedItemElements.Count; i++)
            {
                _itemElementTableById.Add(new ItemId(i), sortedItemElements[i]);
                _itemGuidToItemId.Add(sortedItemElements[i].ItemGuid, new ItemId(i));
            }
        }
        
        public ItemElement GetItemMaster(ItemId itemId)
        {
            if (!_itemElementTableById.TryGetValue(itemId, out var element))
            {
                throw new InvalidOperationException($"ItemElement not found. ItemId:{itemId}");
            }
            return element;
        }
        
        public ItemElement GetItemMaster(Guid itemGuid)
        {
            var itemId = GetItemId(itemGuid);
            return GetItemMaster(itemId);
        }
        
        public ItemId GetItemId(Guid itemGuid)
        {
            if (!_itemGuidToItemId.TryGetValue(itemGuid, out var itemId))
            {
                throw new InvalidOperationException($"ItemElement not found. ItemGuid:{itemGuid}");
            }
            return itemId;
        }
        
        public IEnumerable<ItemId> GetItemIds()
        {
            return _itemElementTableById.Keys;
        }
    }
}