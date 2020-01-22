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
        private LogListener _listener;

        private static DiscordRpc.RichPresence _richPresence = new DiscordRpc.RichPresence();
        private static Models.PoeCharacterInfo _characterInfo;

        public Discord(LogListener listener, ulong appId = 550890770056347648)
        {
            _listener = listener;

            var discordEvents = new DiscordRpc.EventHandlers();
            discordEvents.ReadyCallback += () => Logger.Get.Success("Connected to Discord RPC pipe");
            discordEvents.ErrorCallback += (errCode, msg) => Logger.Get.Error($"Discord error triggered '{errCode}' {msg}");
            discordEvents.DisconnectedCallback += (_, __) => Logger.Get.Info("Disconnected from Discord RPC pipe");

            _callbackRunner = new Timer(2000);
            _callbackRunner.Elapsed += (x, y) => DiscordRpc.Discord_RunCallbacks();
            _callbackRunner.Start();

            _listener.AddListener("] :\\s(.+?) \\((.+)\\) is now level ([0-9]+)", OnLevelUp);
            _listener.AddListener("] : You have entered (.+).", OnInstanceEntered);
            _listener.AddListener("] Connecting to instance server at (.+)", OnInstanceConnect);
            _listener.AddListener("] Connected to (.+) in (.+)ms.", OnLogin);

            DiscordRpc.Discord_Initialize(appId.ToString(), ref discordEvents, false, null);
        }

        #region discord-rpc

        /// <summary>
        /// Gets triggered when the player connects to a new instance
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private static void OnInstanceConnect(string line, Match match)
        {
            Logger.Get.Debug($"Connected to server instance {match.Groups[1]}");

            _characterInfo = PoeApi.GetCharacterData();

            if (_characterInfo is null)
                return;

            _richPresence.Details = $"{_characterInfo.League} League";
            _richPresence.LargeImageKey = _characterInfo.Class.ToLower();
            _richPresence.LargeImageText = $"{_characterInfo.Class} ({_characterInfo.Level})";
        }

        /// <summary>
        /// Gets triggered when the player enters a new instance
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private static void OnInstanceEntered(string line, Match match)
        {
            var location = match.Groups[1].Value;

            Logger.Get.Debug($"Traveled to {location}");

            _richPresence.State = location;
            _richPresence.StartTimestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            _richPresence.Update();
        }

        /// <summary>
        /// Gets triggered when the player levels up
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private static void OnLevelUp(string line, Match match)
        {
            var charName = match.Groups[1].Value;
            var charClass = match.Groups[2].Value;
            var charLevel = match.Groups[3].Value;

            Logger.Get.Debug($"{charName} [{charClass}] is now level {charLevel}");

            if (_characterInfo?.Name != charName)
            {
                // TODO: just update characterInfo instead?
                return;
            }

            _richPresence.LargeImageText = $"{charClass} ({charLevel})";
            _richPresence.Update();
        }

        /// <summary>
        /// Gets triggered when the player logs into poe
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private static void OnLogin(string line, Match match)
        {
            Logger.Get.Debug($"Logged into {match.Groups[1]} with a ping of {match.Groups[2]}ms");
        }

        #endregion discord-rpc

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
                // Game should take care already
                _listener.Dispose();
            }
            DiscordRpc.Discord_Shutdown();

            _disposed = true;
        }
    }
}