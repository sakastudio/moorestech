using System.Collections.Generic;
using Core.Item;
using Game.Block.Config.LoadConfig.Param;
using Game.Block.Interface.BlockConfig;
using MainGame.UnityView.UI.Inventory.Element;
using Server.Protocol.PacketResponse.Util.InventoryMoveUtil;
using UnityEngine;

namespace MainGame.UnityView.UI.Inventory.Sub
{
    public class BlockInventoryView : MonoBehaviour,ISubInventory
    {
        public IReadOnlyList<ItemSlotObject> SubInventorySlotObjects => _blockItemSlotObjects;
        public List<IItemStack> SubInventory { get; private set;}
        public int SubInventorySlotCount => _blockItemSlotObjects.Count;
        public ItemMoveInventoryInfo ItemMoveInventoryInfo { get; private set; }
        
        
        [SerializeField] private ItemSlotObject itemSlotObjectPrefab;
        
        
        [SerializeField] private RectTransform chestItemParent;
        
        
        [SerializeField] private RectTransform minerItemParent;
        
        
        [SerializeField] private RectTransform machineInputItemParent;
        [SerializeField] private RectTransform machineOutputItemParent;
        
        [SerializeField] private RectTransform powerGeneratorFuelItemParent;
        
        private readonly List<ItemSlotObject> _blockItemSlotObjects = new();
        
        
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void SetBlockInventoryType(BlockInventoryType type,Vector2Int blockPos,IBlockConfigParam param)
        {
            ItemMoveInventoryInfo = new ItemMoveInventoryInfo(ItemMoveInventoryType.BlockInventory,blockPos);
            Clear();
            
            switch (type)
            {
                case BlockInventoryType.Chest:
                    Chest();
                    break;
                case BlockInventoryType.Miner:
                    Miner();
                    break;
                case BlockInventoryType.Machine:
                    Machine();
                    break;
                case BlockInventoryType.Generator:
                    Generator();
                    break;
            }
            
            #region Internal

            void Clear()
            {
                foreach (var slotObject in _blockItemSlotObjects)
                {
                    Destroy(slotObject.gameObject);
                }
                _blockItemSlotObjects.Clear();
            }

            void Chest()
            {
                var chestParam = (ChestConfigParam) param;
                for (int i = 0; i < chestParam.ChestItemNum; i++)
                {
                    var slotObject = Instantiate(itemSlotObjectPrefab, chestItemParent);
                    _blockItemSlotObjects.Add(slotObject);
                }
            }
            
            void Miner()
            {
                var minerParam = (MinerBlockConfigParam) param;
                for (int i = 0; i < minerParam.OutputSlot; i++)
                {
                    var slotObject = Instantiate(itemSlotObjectPrefab, minerItemParent);
                    _blockItemSlotObjects.Add(slotObject);
                }
            }
            
            void Machine()
            {
                var machineParam = (MachineBlockConfigParam) param;
                for (int i = 0; i < machineParam.InputSlot; i++)
                {
                    var slotObject = Instantiate(itemSlotObjectPrefab, machineInputItemParent);
                    _blockItemSlotObjects.Add(slotObject);
                }
                for (int i = 0; i < machineParam.OutputSlot; i++)
                {
                    var slotObject = Instantiate(itemSlotObjectPrefab, machineOutputItemParent);
                    _blockItemSlotObjects.Add(slotObject);
                }
            }
            
            void Generator()
            {
                var generatorParam = (PowerGeneratorConfigParam) param;
                for (int i = 0; i < generatorParam.FuelSlot; i++)
                {
                    var slotObject = Instantiate(itemSlotObjectPrefab, powerGeneratorFuelItemParent);
                    _blockItemSlotObjects.Add(slotObject);
                }
            }
            
            #endregion
        }

        public void SetItemList(List<IItemStack> itemStacks)
        {
            SubInventory = itemStacks;
        }

        public void SetItemSlot(IItemStack item, int slot)
        {
            if (SubInventory.Count <= slot)
            {
                Debug.LogError($"インベントリのサイズを超えています。item:{item} slot:{slot}");
                return;
            }
            SubInventory[slot] = item;
        }
    }

    public enum BlockInventoryType
    {
        Chest,
        Miner,
        Machine,
        Generator,
    }
}