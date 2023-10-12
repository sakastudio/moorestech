using System;

namespace Game.PlayerInventory.Interface
{
    public static class PlayerInventoryConst
    {
        public const int MainInventoryColumns = 9;
        public const int MainInventoryRows = 5;
        public const int MainInventorySize = MainInventoryColumns * MainInventoryRows;

        public const int CraftingInventoryColumns = 3;
        public const int CraftingInventoryRows = 3;
        public const int CraftingSlotSize = CraftingInventoryColumns * CraftingInventoryRows;

        /// <summary>
        ///     ホットバーとなるインベントリのスロット
        /// </summary>
        public static readonly int[] HotBarSlots =
        {
            HotBarSlotToInventorySlot(0),
            HotBarSlotToInventorySlot(1),
            HotBarSlotToInventorySlot(2),
            HotBarSlotToInventorySlot(3),
            HotBarSlotToInventorySlot(4),
            HotBarSlotToInventorySlot(5),
            HotBarSlotToInventorySlot(6),
            HotBarSlotToInventorySlot(7),
            HotBarSlotToInventorySlot(8)
        };


        /// <summary>
        ///     0〜8までのホットバーのIDをインベントリのスロットに変換します
        /// </summary>
        /// <exception cref="Exception">0〜8以外だとスローします</exception>
        public static int HotBarSlotToInventorySlot(int slot)
        {
            if (slot < 0 || MainInventoryColumns <= slot)
                throw new Exception("ホットバーは0～8までです");
            //インベントリの一番したがホットバーとなる
            return (MainInventoryRows - 1) * MainInventoryColumns + slot;
        }
    }
}