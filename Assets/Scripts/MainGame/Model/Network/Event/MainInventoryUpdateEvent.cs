using System;
using System.Collections.Generic;
using MainGame.Basic;

namespace MainGame.Model.Network.Event
{
    public class MainInventoryUpdateEvent
    {
        public event Action<MainInventoryUpdateProperties> OnMainInventoryUpdateEvent;
        public event Action<MainInventorySlotUpdateProperties> OnMainInventorySlotUpdateEvent;


        internal void InvokeMainInventoryUpdate(MainInventoryUpdateProperties properties)
        {
            OnMainInventoryUpdateEvent?.Invoke(properties);
        }

        internal void InvokeMainInventorySlotUpdate(MainInventorySlotUpdateProperties properties)
        {
            OnMainInventorySlotUpdateEvent?.Invoke(properties);
        }
    }
    
    

    public class MainInventoryUpdateProperties
    {
        public readonly int PlayerId;
        public readonly List<ItemStack> ItemStacks;

        public MainInventoryUpdateProperties(int playerId, List<ItemStack> itemStacks)
        {
            PlayerId = playerId;
            ItemStacks = itemStacks;
        }
    }

    public class MainInventorySlotUpdateProperties
    {
        public readonly int SlotId;
        public readonly ItemStack ItemStack;

        public MainInventorySlotUpdateProperties(int slotId, ItemStack itemStack)
        {
            SlotId = slotId;
            ItemStack = itemStack;
        }
    }
}