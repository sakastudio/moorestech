﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.PlayerInventory.Interface;
using MessagePack;
using Server.Event;
using Server.Protocol.Base;
using Server.Util;
using Server.Util.MessagePack;

namespace Server.Protocol.PacketResponse
{
    public class PlayerInventoryResponseProtocol : IPacketResponse
    {
        public const string Tag = "va:playerInvRequest";
        
        private IPlayerInventoryDataStore _playerInventoryDataStore;

        public PlayerInventoryResponseProtocol(IPlayerInventoryDataStore playerInventoryDataStore)
        {
            _playerInventoryDataStore = playerInventoryDataStore;
        }

        public List<ToClientProtocolMessagePackBase> GetResponse(List<byte> payload)
        {
            var data = MessagePackSerializer.Deserialize<RequestPlayerInventoryProtocolMessagePack>(payload.ToArray());
            
            var playerInventory = _playerInventoryDataStore.GetInventoryData(data.PlayerId);

            //ExportInventoryLog(playerInventory);

            //メインインベントリのアイテムを設定
            var mainItems = new List<ItemMessagePack>();
            for (int i = 0; i < PlayerInventoryConst.MainInventorySize; i++)
            {
                var id = playerInventory.MainOpenableInventory.GetItem(i).Id;
                var count = playerInventory.MainOpenableInventory.GetItem(i).Count;
                mainItems.Add(new ItemMessagePack(id,count));
            }
            
            
            //グラブインベントリのアイテムを設定
            var grabItem = new ItemMessagePack(
                playerInventory.GrabInventory.GetItem(0).Id, 
                playerInventory.GrabInventory.GetItem(0).Count);

            
            //クラフトインベントリのアイテムを設定
            var craftItems = new List<ItemMessagePack>();
            for (int i = 0; i < PlayerInventoryConst.CraftingSlotSize; i++)
            {
                var id = playerInventory.CraftingOpenableInventory.GetItem(i).Id;
                var count = playerInventory.CraftingOpenableInventory.GetItem(i).Count;
                craftItems.Add(new ItemMessagePack(id,count));
            }
            
            //クラフト結果のアイテムを設定
            var craftItem = new ItemMessagePack(
                playerInventory.CraftingOpenableInventory.GetCreatableItem().Id, 
                playerInventory.CraftingOpenableInventory.GetCreatableItem().Count);
            
            var isCreatable = playerInventory.CraftingOpenableInventory.IsCreatable();

            var response = new PlayerInventoryResponseProtocolMessagePack(
                data.PlayerId,mainItems.ToArray(),grabItem,craftItems.ToArray(),craftItem,isCreatable);
            

            return new List<ToClientProtocolMessagePackBase>() {response};
        }


        /// <summary>
        /// デバッグ用でインベントリの中身が知りたい時に使用する 
        /// </summary>
        public static void ExportInventoryLog(PlayerInventoryData playerInventory,bool isExportMain,bool isExportCraft,bool isExportGrab)
        {
            var inventoryStr = new StringBuilder();
            inventoryStr.AppendLine("Main Inventory");


            if (isExportMain)
            {
                //メインインベントリのアイテムを設定
                for (int i = 0; i < PlayerInventoryConst.MainInventorySize; i++)
                {
                    var id = playerInventory.MainOpenableInventory.GetItem(i).Id;
                    var count = playerInventory.MainOpenableInventory.GetItem(i).Count;

                    inventoryStr.Append(id + " " + count + "  ");
                    if ((i + 1) % PlayerInventoryConst.MainInventoryColumns == 0)
                    {
                        inventoryStr.AppendLine();
                    }
                }
            }

            inventoryStr.AppendLine();

            if (isExportGrab)
            {
                inventoryStr.AppendLine("Grab Inventory");
                inventoryStr.AppendLine(playerInventory.GrabInventory.GetItem(0).Id + " " + playerInventory.GrabInventory.GetItem(0).Count + "  ");
            }


            if (isExportCraft)
            {
                inventoryStr.AppendLine();
                inventoryStr.AppendLine("Craft Inventory");
                //クラフトインベントリのアイテムを設定
                for (int i = 0; i < PlayerInventoryConst.CraftingSlotSize; i++)
                {
                    var id = playerInventory.CraftingOpenableInventory.GetItem(i).Id;
                    var count = playerInventory.CraftingOpenableInventory.GetItem(i).Count;

                    inventoryStr.Append(id + " " + count + "  ");
                    if ((i + 1) % PlayerInventoryConst.CraftingInventoryColumns == 0)
                    {
                        inventoryStr.AppendLine();
                    }
                }
                inventoryStr.AppendLine("Craft Result Item");
                inventoryStr.AppendLine(playerInventory.CraftingOpenableInventory.GetCreatableItem().Id + " " + playerInventory.CraftingOpenableInventory.GetCreatableItem().Count + "  ");
            }
            
            Console.WriteLine(inventoryStr);
        }
    }
    
    
    [MessagePackObject(keyAsPropertyName :true)]
    public class RequestPlayerInventoryProtocolMessagePack : ToServerProtocolMessagePackBase
    {
        [Obsolete("デシリアライズ用のコンストラクタです。基本的に使用しないでください。")]
        public RequestPlayerInventoryProtocolMessagePack() { }

        public RequestPlayerInventoryProtocolMessagePack(int playerId)
        {
            ToServerTag = PlayerInventoryResponseProtocol.Tag;
            PlayerId = playerId;
        }

        public int PlayerId { get; set; }
    }
    
    
    [MessagePackObject(keyAsPropertyName :true)]
    public class PlayerInventoryResponseProtocolMessagePack : ToClientProtocolMessagePackBase
    {
        [Obsolete("デシリアライズ用のコンストラクタです。基本的に使用しないでください。")]
        public PlayerInventoryResponseProtocolMessagePack() { }


        public PlayerInventoryResponseProtocolMessagePack(int playerId, ItemMessagePack[] main, ItemMessagePack grab, ItemMessagePack[] craft, ItemMessagePack craftResult, bool isCreatable)
        {
            ToClientTag = PlayerInventoryResponseProtocol.Tag;
            PlayerId = playerId;
            Main = main;
            Grab = grab;
            Craft = craft;
            CraftResult = craftResult;
            IsCreatable = isCreatable;
        }

        public int PlayerId { get; set; }
        
        public ItemMessagePack[] Main { get; set; }
        public ItemMessagePack Grab { get; set; }
        
        public ItemMessagePack[] Craft { get; set; }
        public ItemMessagePack CraftResult { get; set; }
        public bool IsCreatable { get; set; }
    }
}