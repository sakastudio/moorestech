using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Core.ConfigJson;
using Core.Item;
using Game.Crafting.Interface;
using Newtonsoft.Json;

namespace Game.Crafting.Config
{
    public class CraftConfigJsonLoad
    {
        private readonly ItemStackFactory _itemStackFactory;

        public CraftConfigJsonLoad(ItemStackFactory itemStackFactory)
        {
            _itemStackFactory = itemStackFactory;
        }

        public List<CraftingConfigData> Load(List<string> jsons)
        {
            var loadedData = jsons.SelectMany(JsonConvert.DeserializeObject<CraftConfigDataElement[]>).ToList();
            
            var result = new List<CraftingConfigData>();

            for (var i = 0; i < loadedData.Count; i++)
            {
                var config = loadedData[i];
                var items = new List<IItemStack>();
                foreach (var craftItem in config.Items)
                {
                    if (string.IsNullOrEmpty(craftItem.ItemName) || string.IsNullOrEmpty(craftItem.ModId))
                    {
                        items.Add(_itemStackFactory.CreatEmpty());
                        continue;
                    }

                    items.Add(_itemStackFactory.Create(craftItem.ModId, craftItem.ItemName, craftItem.Count));
                }

                //TODO ロードした時にあるべきものがなくnullだったらエラーを出す
                if (config.Result.ModId == null)
                {
                    Console.WriteLine(i + " : Result item is null");
                }

                var resultItem =
                    _itemStackFactory.Create(config.Result.ModId, config.Result.ItemName, config.Result.Count);

                result.Add(new CraftingConfigData(items, resultItem));
            }

            return result;
        }
    }
}