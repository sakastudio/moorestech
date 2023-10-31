﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Update;
using Game.Save.Interface;
using Microsoft.Extensions.DependencyInjection;
using Mod.Base;
using Mod.Loader;
using Server.Boot.PacketHandle;

namespace Server.Boot
{
    public static class StartServer
    {
        private const int argsCount = 1;


        private static string DebugServerDirectory
        {
            get
            {
                var path = Environment.GetEnvironmentVariable("MOORES_SERVER_DIRECTORY");
                if (path != null) return path;

                //環境変数を取得する
                Console.WriteLine("環境変数にコンフィグのパスが指定されていませんでした。MOORES_SERVER_DIRECTORYを設定してください。");
                Console.WriteLine("Windowsの場合の設定コマンド > setx /M MOORES_SERVER_DIRECTORY \"C:～ \"");
                Console.WriteLine("Macの場合の設定コマンド > export MOORES_SERVER_DIRECTORY=\"～\"");
                return Environment.CurrentDirectory;
            }
        }

        private static string StartupFromClientFolderPath
        {
            get
            {
                var di = new DirectoryInfo(Environment.CurrentDirectory);
                return Path.Combine(di.FullName, "server", "mods");
            }
        }

        public static async Task Start(string[] args)
        {
            try
            {
#if DEBUG
                var serverDirectory = DebugServerDirectory;
#else
                var serverDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

                Console.WriteLine("データをロードします　パス:" + serverDirectory);

                var (packet, serviceProvider) = new PacketResponseCreatorDiContainerGenerators().Create(serverDirectory);

                //マップをロードする
                serviceProvider.GetService<IWorldSaveDataLoader>().LoadOrInitialize();

                //modのOnLoadコードを実行する
                var modsResource = serviceProvider.GetService<ModsResource>();
                modsResource.Mods.ToList().ForEach(
                    m => m.Value.ModEntryPoints.ForEach(
                        e =>
                        {
                            Console.WriteLine("Modをロードしました modId:" + m.Value + " className:" + e.GetType().Name);
                            e.OnLoad(new ServerModEntryInterface(serviceProvider, packet));
                        }));


                //サーバーの起動とゲームアップデートの開始
                new Thread(() => new PacketHandler().StartServer(packet)).Start();
                new Thread(() =>
                {
                    while (true) GameUpdater.Update();
                }).Start();

                await new AutoSaveSystem(serviceProvider.GetService<IWorldSaveDataSaver>()).AutoSave();

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("StackTrace");
                Console.WriteLine(e.StackTrace);

                Console.WriteLine();
                Console.WriteLine("Message");

                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
    }
}