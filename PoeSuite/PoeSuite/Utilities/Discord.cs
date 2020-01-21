using PoeSuite.Imports;
using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PoeSuite.Utilities
{
    class Discord
    {
        private Timer _callbackRunner;

        public Discord(ulong appId = 550890770056347648)
        {
            var discordEvents = new DiscordRpc.EventHandlers();
            discordEvents.ReadyCallback += () => Logger.Get.Success("Connected to Discord RPC pipe");
            discordEvents.ErrorCallback += (errCode, msg) => Logger.Get.Error($"Discord error triggered '{errCode}' {msg}");
            discordEvents.DisconnectedCallback += (_, __) => Logger.Get.Info("Disconnected from Discord RPC pipe");


            _callbackRunner = new Timer(2000);
            _callbackRunner.Elapsed += (x, y) => DiscordRpc.Discord_RunCallbacks();
            _callbackRunner.Start();

            DiscordRpc.Discord_Initialize(appId.ToString(), ref discordEvents, false, null);
        }
    }
}
