using MainGame.Basic;
using MainGame.GameLogic.Inventory;
using MainGame.UnityView.UI.Inventory.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MainGame.Control.UI.Inventory
{
    public class BlockInventoryInput : MonoBehaviour,IPostStartable
    {
        private int _equippedItemIndex = -1;
        private BlockInventoryItemView _blockInventoryItemView;
        private InventoryItemMoveService _inventoryItemMoveService;
        private BlockInventoryDataCache _blockInventoryDataCache;
        private BlockInventoryEquippedItemImageSet _blockInventoryEquippedItemImageSet;
        
        private MoorestechInputSettings _inputSettings;



        [Inject]
        public void Construct(
            BlockInventoryItemView blockInventoryItemView,
            InventoryItemMoveService inventoryItemMoveService,
            BlockInventoryDataCache blockInventoryDataCache,
            BlockInventoryEquippedItemImageSet blockInventoryEquippedItemImageSet)
        {
            _blockInventoryItemView = blockInventoryItemView;
            _inventoryItemMoveService = inventoryItemMoveService;
            _blockInventoryDataCache = blockInventoryDataCache;
            _blockInventoryEquippedItemImageSet = blockInventoryEquippedItemImageSet;
            
            _blockInventoryEquippedItemImageSet.gameObject.SetActive(false);
            _inputSettings = new();
            _inputSettings.Enable();
        }
        
        
        
        //イベントをボタンに登録する
        public void PostStart()
        {
            foreach (var slot in _blockInventoryItemView.GetAllInventoryItemSlots())
            {
                slot.SubscribeOnItemSlotClick(OnSlotClick);
            }
        }
        
        
        
        //ボタンがクリックされた時に呼び出される
        private void OnSlotClick(int slot)
        {
            var isSlotEmpty = _blockInventoryDataCache.GetItemStack(slot).ID == ItemConstant.NullItemId;
            if (_equippedItemIndex == -1 && !isSlotEmpty)
            {
                _equippedItemIndex = slot;
                //アイテムをクリックしたときに追従する画像の設定
                _blockInventoryEquippedItemImageSet.SetEquippedItemIndex(slot);
                _blockInventoryEquippedItemImageSet.gameObject.SetActive(true);
                return;
            }

            var fromSlot = _equippedItemIndex;
            var fromIsBlock = false;
            var toSlot = slot;
            var toIsBlock = false;
            //slot数がプレイヤーインベントリのslot数よりも多いときはブロックないのインベントリと判断する
            if (PlayerInventoryConstant.MainInventorySize <= fromSlot)
            {
                fromSlot -= PlayerInventoryConstant.MainInventorySize;
                fromIsBlock = true;
            }
            if (PlayerInventoryConstant.MainInventorySize <= toSlot)
            {
                toSlot -= PlayerInventoryConstant.MainInventorySize;
                toIsBlock = true;
            }
            
            //アイテムを半分だけおく
            if (_inputSettings.UI.InventoryItemHalve.inProgress)
            {
               _inventoryItemMoveService.MoveHalfItemStack(fromSlot,fromIsBlock,toSlot,toIsBlock);
                return;
            }
            
            //アイテムを一個だけおく
            if (_inputSettings.UI.InventoryItemOnePut.inProgress)
            {
                _inventoryItemMoveService.MoveOneItemStack(fromSlot,fromIsBlock,toSlot,toIsBlock);
                return;
            }
            
            //アイテムを全部おく
            _inventoryItemMoveService.MoveAllItemStack(fromSlot,fromIsBlock,toSlot,toIsBlock);
            _equippedItemIndex = -1;
            _blockInventoryEquippedItemImageSet.gameObject.SetActive(false);
        }
    }
}