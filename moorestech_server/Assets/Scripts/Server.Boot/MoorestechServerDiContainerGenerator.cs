using System.Collections.Generic;
using System.IO;
using Core.ConfigJson;
using Core.EnergySystem;
using Core.EnergySystem.Electric;
using Core.Item;
using Core.Item.Config;
using Game.Block.Component;
using Game.Block.Config;
using Game.Block.Event;
using Game.Block.Factory;
using Game.Block.Interface;
using Game.Block.Interface.BlockConfig;
using Game.Block.Interface.Event;
using Game.Block.Interface.RecipeConfig;
using Game.Block.RecipeConfig;
using Game.Crafting.Config;
using Game.Crafting.Interface;
using Game.Entity;
using Game.Entity.Interface;
using Game.Map;
using Game.Map.Interface;
using Game.Map.Interface.Json;
using Game.Map.Interface.Vein;
using Game.PlayerInventory;
using Game.PlayerInventory.Event;
using Game.PlayerInventory.Interface;
using Game.PlayerInventory.Interface.Event;
using Game.SaveLoad.Interface;
using Game.SaveLoad.Json;
using Game.World;
using Game.World.DataStore;
using Game.World.DataStore.WorldSettings;
using Game.World.Event;
using Game.World.EventHandler.EnergyEvent;
using Game.World.EventHandler.EnergyEvent.EnergyService;
using Game.World.Interface;
using Game.World.Interface.DataStore;
using Game.World.Interface.Event;
using Game.WorldMap.EventListener;
using Microsoft.Extensions.DependencyInjection;
using Mod.Config;
using Newtonsoft.Json;
using Server.Event;
using Server.Event.EventReceive;
using Server.Protocol;

namespace Server.Boot
{
    public class MoorestechServerDiContainerGenerator
    {
        //TODO セーブファイルのディレクトリもここで指定できるようにする
        public (PacketResponseCreator, ServiceProvider) Create(string serverDirectory)
        {
            var services = new ServiceCollection();

            var modDirectory = Path.Combine(serverDirectory, "mods");
            var mapPath = Path.Combine(serverDirectory, "map", "map.json");

            //コンフィグ、ファクトリーのインスタンスを登録
            (Dictionary<string, ConfigJson> configJsons, var modsResource) = ModJsonStringLoader.GetConfigString(modDirectory);
            services.AddSingleton(new ConfigJsonList(configJsons));
            services.AddSingleton(modsResource);
            services.AddSingleton<IMachineRecipeConfig, MachineRecipeConfig>();
            services.AddSingleton<IItemConfig, ItemConfig>();
            services.AddSingleton<ICraftingConfig, CraftConfig>();
            services.AddSingleton<ItemStackFactory, ItemStackFactory>();
            services.AddSingleton<IBlockConfig, BlockConfig>();
            services.AddSingleton<VanillaIBlockTemplates, VanillaIBlockTemplates>();
            services.AddSingleton<IBlockFactory, BlockFactory>();
            services.AddSingleton<ComponentFactory, ComponentFactory>();


            //ゲームプレイに必要なクラスのインスタンスを生成
            services.AddSingleton<EventProtocolProvider, EventProtocolProvider>();
            services.AddSingleton<IWorldSettingsDatastore, WorldSettingsDatastore>();
            services.AddSingleton<IWorldBlockDatastore, WorldBlockDatastore>();
            services.AddSingleton<IPlayerInventoryDataStore, PlayerInventoryDataStore>();
            services.AddSingleton<IWorldBlockUpdateEvent, WorldBlockUpdateEvent>();
            services.AddSingleton<IBlockInventoryOpenStateDataStore, BlockInventoryOpenStateDataStore>();
            services.AddSingleton<IWorldEnergySegmentDatastore<EnergySegment>, WorldEnergySegmentDatastore<EnergySegment>>();
            services.AddSingleton<MaxElectricPoleMachineConnectionRange, MaxElectricPoleMachineConnectionRange>();
            services.AddSingleton<IEntitiesDatastore, EntitiesDatastore>();
            services.AddSingleton<IEntityFactory, EntityFactory>();

            services.AddSingleton<IMapObjectDatastore, MapObjectDatastore>();
            services.AddSingleton<IMapObjectFactory, MapObjectFactory>();
            services.AddSingleton<IMapVeinDatastore, MapVeinDatastore>();


            //JSONファイルのセーブシステムの読み込み
            services.AddSingleton<IWorldSaveDataSaver, WorldSaverForJson>();
            services.AddSingleton<IWorldSaveDataLoader, WorldLoaderFromJson>();
            services.AddSingleton(new SaveJsonFileName("save_1.json"));
            services.AddSingleton(JsonConvert.DeserializeObject<MapInfoJson>(File.ReadAllText(mapPath)));

            //イベントを登録
            services.AddSingleton<IBlockPlaceEvent, BlockPlaceEvent>();
            services.AddSingleton<IBlockRemoveEvent, BlockRemoveEvent>();
            services.AddSingleton<IBlockOpenableInventoryUpdateEvent, BlockOpenableInventoryUpdateEvent>();
            services.AddSingleton<IMainInventoryUpdateEvent, MainInventoryUpdateEvent>();
            services.AddSingleton<IGrabInventoryUpdateEvent, GrabInventoryUpdateEvent>();

            //イベントレシーバーを登録
            services.AddSingleton<ChangeBlockStateEventPacket>();
            services.AddSingleton<MainInventoryUpdateEventPacket>();
            services.AddSingleton<OpenableBlockInventoryUpdateEventPacket>();
            services.AddSingleton<GrabInventoryUpdateEventPacket>();
            services.AddSingleton<PlaceBlockEventPacket>();
            services.AddSingleton<RemoveBlockToSetEventPacket>();

            services.AddSingleton<EnergyConnectUpdaterContainer<EnergySegment, IBlockElectricConsumer, IElectricGenerator, IElectricPole>>();

            services.AddSingleton<SetMiningItemToMiner>();
            services.AddSingleton<MapObjectUpdateEventPacket>();

            //データのセーブシステム
            services.AddSingleton<AssembleSaveJsonText, AssembleSaveJsonText>();


            var serviceProvider = services.BuildServiceProvider();
            var packetResponse = new PacketResponseCreator(serviceProvider);

            //イベントレシーバーをインスタンス化する
            //TODO この辺を解決するDIコンテナを探す VContinerのRegisterEntryPoint的な
            serviceProvider.GetService<MainInventoryUpdateEventPacket>();
            serviceProvider.GetService<OpenableBlockInventoryUpdateEventPacket>();
            serviceProvider.GetService<GrabInventoryUpdateEventPacket>();
            serviceProvider.GetService<PlaceBlockEventPacket>();
            serviceProvider.GetService<RemoveBlockToSetEventPacket>();

            serviceProvider.GetService<EnergyConnectUpdaterContainer<EnergySegment, IBlockElectricConsumer, IElectricGenerator, IElectricPole>>();

            serviceProvider.GetService<SetMiningItemToMiner>();
            serviceProvider.GetService<ChangeBlockStateEventPacket>();
            serviceProvider.GetService<MapObjectUpdateEventPacket>();

            return (packetResponse, serviceProvider);
        }
    }
}