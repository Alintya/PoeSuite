using PoeSuite.Imports;
using System;
using System.Text.RegularExpressions;
using System.Timers;

namespace PoeSuite.Utilities
{
    internal class Discord : IDisposable
    {
        private bool _disposed;
        private Timer _callbackRunner;

        public DiscordRpc.RichPresence RichPresenceData = new DiscordRpc.RichPresence();

        public Discord(LogListener listener, ulong appId = 550890770056347648)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {

            }
            DiscordRpc.Discord_Shutdown();

            _disposed = true;
        }
    }
}