using System.IO;
using Core.Block.BlockFactory;
using Core.Block.BlockInventory;
using Core.Block.Blocks.Miner;
using Core.Block.Config;
using Core.Block.Config.Service;
using Core.Block.Event;
using Core.Block.RecipeConfig;
using Core.ConfigJson;
using Core.EnergySystem;
using Core.EnergySystem.Electric;
using Core.Inventory;
using Core.Item;
using Core.Item.Config;
using Core.Ore;
using Core.Ore.Config;
using Game.Crafting;
using Game.Crafting.Config;
using Game.Crafting.Interface;
using Game.Entity;
using Game.Entity.Interface;
using Game.MapObject;
using Game.MapObject.Interface;
using Game.PlayerInventory.Interface;
using Game.PlayerInventory.Interface.Event;
using Game.Quest;
using Game.Quest.Config;
using Game.Quest.Event;
using Game.Quest.Factory;
using Game.Quest.Interface;
using Game.Quest.Interface.Event;
using Game.Save.Interface;
using Game.Save.Json;
using Game.World.EventHandler;
using Game.World.EventHandler.EnergyEvent;
using Game.World.EventHandler.InventoryEvent;
using Game.World.EventHandler.Service;
using Game.World.Interface.DataStore;
using Game.World.Interface.Event;
using Game.WorldMap;
using Game.WorldMap.EventListener;
using Microsoft.Extensions.DependencyInjection;
using Mod.Config;
using PlayerInventory;
using PlayerInventory.Event;
using Server.Event;
using Server.Event.EventReceive;
using Server.Protocol;
using World.DataStore;
using World.DataStore.WorldSettings;
using World.Event;

namespace Server.Boot
{
    public class PacketResponseCreatorDiContainerGenerators
    {
        //TODO セーブファイルのディレクトリもここで指定できるようにする
        public (PacketResponseCreator, ServiceProvider) Create(string serverDirectory)
        {
            var services = new ServiceCollection();
            
            var modDirectory = Path.Combine(serverDirectory, "mods");
            var mapDirectory = Path.Combine(serverDirectory, "map");
            
            //コンフィグ、ファクトリーのインスタンスを登録
            var (configJsons,modsResource) = ModJsonStringLoader.GetConfigString(modDirectory);
            services.AddSingleton(new ConfigJsonList(configJsons));
            services.AddSingleton(modsResource);
            services.AddSingleton<IMachineRecipeConfig, MachineRecipeConfig>();
            services.AddSingleton<IItemConfig, ItemConfig>();
            services.AddSingleton<ICraftingConfig, CraftConfig>();
            services.AddSingleton<ItemStackFactory, ItemStackFactory>();
            services.AddSingleton<IBlockConfig, BlockConfig>();
            services.AddSingleton<VanillaIBlockTemplates, VanillaIBlockTemplates>();
            services.AddSingleton<BlockFactory, BlockFactory>();
            services.AddSingleton<ItemIdToBlockId, ItemIdToBlockId>();


            //ゲームプレイに必要なクラスのインスタンスを生成
            services.AddSingleton<IIsCreatableJudgementService, IsCreatableJudgementService>();
            services.AddSingleton<EventProtocolProvider, EventProtocolProvider>();
            services.AddSingleton<IWorldSettingsDatastore, WorldSettingsDatastore>();
            services.AddSingleton<IWorldBlockDatastore, WorldBlockDatastore>();
            services.AddSingleton<IPlayerInventoryDataStore, PlayerInventoryDataStore>();
            services.AddSingleton<IBlockInventoryOpenStateDataStore, BlockInventoryOpenStateDataStore>();
            services.AddSingleton<IWorldEnergySegmentDatastore<EnergySegment>, WorldEnergySegmentDatastore<EnergySegment>>();
            services.AddSingleton<MaxElectricPoleMachineConnectionRange, MaxElectricPoleMachineConnectionRange>();
            services.AddSingleton<IOreConfig, OreConfig>();
            services.AddSingleton<VeinGenerator, VeinGenerator>();
            services.AddSingleton<WorldMapTile, WorldMapTile>();
            services.AddSingleton(new Seed(1337));
            services.AddSingleton<IEntitiesDatastore, EntitiesDatastore>();
            services.AddSingleton<IEntityFactory, EntityFactory>();
            services.AddSingleton<IQuestDataStore, QuestDatastore>();
            services.AddSingleton<IQuestConfig, QuestConfig>();
            services.AddSingleton<QuestFactory, QuestFactory>();
            
            services.AddSingleton<IMapObjectDatastore, MapObjectDatastore>();
            services.AddSingleton<IMapObjectFactory, MapObjectFactory>();
            

            //JSONファイルのセーブシステムの読み込み
            services.AddSingleton<IWorldSaveDataSaver, WorldSaverForJson>();
            services.AddSingleton<IWorldSaveDataLoader, WorldLoaderFromJson>();
            services.AddSingleton(new SaveJsonFileName("save_1.json"));
            services.AddSingleton(new MapConfigFile(mapDirectory));

            //イベントを登録
            services.AddSingleton<IBlockPlaceEvent, BlockPlaceEvent>();
            services.AddSingleton<IBlockRemoveEvent, BlockRemoveEvent>();
            services.AddSingleton<IBlockOpenableInventoryUpdateEvent, BlockOpenableInventoryUpdateEvent>();
            services.AddSingleton<IMainInventoryUpdateEvent, MainInventoryUpdateEvent>();
            services.AddSingleton<ICraftInventoryUpdateEvent, CraftInventoryUpdateEvent>();
            services.AddSingleton<IGrabInventoryUpdateEvent, GrabInventoryUpdateEvent>();
            services.AddSingleton<ICraftingEvent, CraftingEvent>();
            services.AddSingleton<IQuestCompletedEvent, QuestCompletedEvent>();

            //イベントレシーバーを登録
            services.AddSingleton<ChangeBlockStateEventPacket>();
            services.AddSingleton<MainInventoryUpdateToSetEventPacket>();
            services.AddSingleton<CraftingInventoryUpdateToSetEventPacket>();
            services.AddSingleton<OpenableBlockInventoryUpdateToSetEventPacket>();
            services.AddSingleton<GrabInventoryUpdateToSetEventPacket>();
            services.AddSingleton<PlaceBlockToSetEventPacket>();
            services.AddSingleton<RemoveBlockToSetEventPacket>();
            services.AddSingleton<BlockPlaceEventToBlockInventoryConnect>();
            services.AddSingleton<BlockRemoveEventToBlockInventoryDisconnect>();
            
            services.AddSingleton<EnergyConnectUpdaterContainer<EnergySegment,IBlockElectricConsumer,IPowerGenerator,IElectricPole>>();
            
            services.AddSingleton<SetMiningItemToMiner>();
            services.AddSingleton<QuestCompletedToSendEventPacket>();
            services.AddSingleton<MapObjectUpdateEventPacket>();

            //データのセーブシステム
            services.AddSingleton<AssembleSaveJsonText, AssembleSaveJsonText>();


            var serviceProvider = services.BuildServiceProvider();
            var packetResponse = new PacketResponseCreator(serviceProvider);

            //イベントレシーバーをインスタンス化する
            //TODO この辺を解決するDIコンテナを探す VContinerのRegisterEntryPoint的な
            serviceProvider.GetService<MainInventoryUpdateToSetEventPacket>();
            serviceProvider.GetService<CraftingInventoryUpdateToSetEventPacket>();
            serviceProvider.GetService<OpenableBlockInventoryUpdateToSetEventPacket>();
            serviceProvider.GetService<GrabInventoryUpdateToSetEventPacket>();
            serviceProvider.GetService<PlaceBlockToSetEventPacket>();
            serviceProvider.GetService<RemoveBlockToSetEventPacket>();
            serviceProvider.GetService<BlockPlaceEventToBlockInventoryConnect>();
            serviceProvider.GetService<BlockRemoveEventToBlockInventoryDisconnect>();
            
            serviceProvider.GetService<EnergyConnectUpdaterContainer<EnergySegment,IBlockElectricConsumer,IPowerGenerator,IElectricPole>>();
            
            serviceProvider.GetService<SetMiningItemToMiner>();
            serviceProvider.GetService<QuestCompletedToSendEventPacket>();
            serviceProvider.GetService<ChangeBlockStateEventPacket>();
            serviceProvider.GetService<MapObjectUpdateEventPacket>();

            return (packetResponse, serviceProvider);
        }
    }
}