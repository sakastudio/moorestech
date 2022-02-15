using Core.Inventory;
using Core.Item;

namespace Server.Protocol.PacketResponse.Util
{
    public class InventoryItemMove
    {
        public void Move(ItemStackFactory itemStackFactory, IInventory sourceInventory, int sourceSlot,
            IInventory destinationInventory, int destinationSlot, int itemCount)
        {
            //移動元と移動先のIDが同じ場合は移動しない
            if (sourceInventory.GetHashCode() == destinationInventory.GetHashCode() && sourceSlot == destinationSlot)
            {
                return;
            }
            
            
            //移動元からアイテムを取得
            var originItem = sourceInventory.GetItem(sourceSlot);
            //移動アイテム数が本来のアイテムより多い時は、本来のアイテム数に修正する
            if (originItem.Count < itemCount)
            {
                itemCount = originItem.Count;
            }

            //実際に移動するアイテムインスタンスの作成
            var moveItem = itemStackFactory.Create(originItem.Id, itemCount);

            var destinationInventoryItem = destinationInventory.GetItem(destinationSlot);

            //移動先アイテムがなかった時はそのまま入れかえる
            //移動先と同じIDの時は移動先スロットに加算し、余ったアイテムを移動元インベントリに入れる
            if (destinationInventoryItem.Count == 0 || originItem.Id == destinationInventoryItem.Id)
            {
                //移動先インベントリにアイテムを移動
                var replaceItem = destinationInventory.ReplaceItem(destinationSlot, moveItem);

                //移動元インベントリに残るアイテムを計算
                //ゼロの時は自動でNullItemになる
                var playerItemCount = originItem.Count - itemCount;
                var remainItem = replaceItem.AddItem(itemStackFactory.Create(originItem.Id, playerItemCount))
                    .ProcessResultItemStack;

                //移動元インベントリに残りのアイテムをセット
                sourceInventory.SetItem(sourceSlot, remainItem);
            }
            //移動元と移動先のIDが異なる時、移動元インベントリのアイテムをすべて入れ替える時にのみ入れ替えを実行する
            //一部入れ替え時は入れ替え作業は実行しない
            else if (itemCount == originItem.Count)
            {
                destinationInventory.SetItem(destinationSlot, originItem);
                sourceInventory.SetItem(sourceSlot, destinationInventoryItem);
            }
        }
    }
}