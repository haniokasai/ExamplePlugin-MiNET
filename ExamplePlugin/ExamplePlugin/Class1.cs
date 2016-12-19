using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Plugins.Commands;
using Newtonsoft.Json;
using System;

namespace ExamplePlugin
{
    [Plugin(PluginName = "ExamplePlugin", Description = "", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : Plugin
    {
        protected override void OnEnable()
        {
            Context.PluginManager.LoadCommands(new HelpCommand(Context.Server.PluginManager));// /helpを使えるようにする
            Context.PluginManager.LoadCommands(new VanillaCommands(Context.Server.PluginManager));// /opを使えるようにする
            Console.Write("Loaded");

            //主にイベントはOnEnableで定義します
            //https://github.com/Ogiwara-CostlierRain464/Csharp/blob/5c2eef5ed74761f32013ad35ab7d2c5b127f20ae/MiNETplugins/TestPlugin/TestPlugin/Class1.cs
            var server = Context.Server;

            server.PlayerFactory.PlayerCreated += PlayerFactory_PlayerCreated;


        }

        //イベント

        private void PlayerFactory_PlayerCreated(object sender, PlayerEventArgs e)
        {
            var player = e.Player;
            player.PlayerJoin += Player_PlayerJoin;
            player.PlayerLeave += Player_PlayerLeave;
            player.HealthManager.PlayerTakeHit += Player_PlayerTakeHit;
            
           // player.HealthManager.PlayerTakeHitでその都度死んだか確認、、、、
        }

        private void Player_PlayerTakeHit(object sender, HealthEventArgs e)
        {
            Player player = (Player)e.SourceEntity;//攻撃する法
            Player player2 = (Player)e.TargetEntity;//受けるほう
            player.SendMessage("you are source");
            player2.SendMessage("you are target");
            player.Level.BroadcastMessage("isdead : "+ player2.HealthManager.IsDead+" health : "+ player2.HealthManager.Health+" hearts : "+ player2.HealthManager.Hearts);//healthかheartsが0ならば死んだ。

        }

        private void Player_PlayerLeave(object sender, PlayerEventArgs e)
        {
            Player player = e.Player;
            if (player == null) throw new ArgumentNullException(nameof(e.Player));
            player.Level.BroadcastMessage("Bye");
            
        }


        private void Player_PlayerJoin(object sender, PlayerEventArgs e)
        {
            Player player = e.Player;
            player.Level.BroadcastMessage("Welcome");
            player.SetGameMode(MiNET.Worlds.GameMode.Survival);
            var setCmdEnabled = McpeSetCommandsEnabled.CreateObject();
            setCmdEnabled.enabled = true;
            player.SendPackage(setCmdEnabled);


            PluginManager pm = new PluginManager();
            pm.HandleCommand(null, "help", "default",null);
        }

        //疑似コマンド関数
        [PacketHandler]
        public void OnChat(McpeText packet,Player player)
        {
            switch(packet.message)
            {
                case "run":
                    //run(player);
                    break;
            }
        }

        //コマンド
        [Command(Name = "test")]
        public void test(Player player)
        {
            player.Level.BroadcastMessage("test", type: MessageType.Raw); // Message typeはtip popup messageが選べる！
        }

        


    }
}
