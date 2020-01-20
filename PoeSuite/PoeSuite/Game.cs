using PoeSuite.Imports;
using PoeSuite.Models;
using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static PoeSuite.Imports.Iphlpapi;

namespace PoeSuite
{
    public class Game : IDisposable
    {
        private static readonly string[] _executableNames =
        {
            "PathOfExile",
            "PathOfExile_x64",
            "PathOfExileSteam",
            "PathOfExile_x64Steam",
            "PathOfExile_KG",
            "PathOfExile_x64_KG"
        };

        //private static readonly object _padlock = new object();
        //private static Game _instance = null;

        private Process _proc;
        private bool _disposed;
        private static DiscordRpc.RichPresence _richPresence = new DiscordRpc.RichPresence();
        private static PoeCharacterInfo _characterInfo;

        public event EventHandler GameProcessExited;

        public string LogFile => Path.Combine(Path.GetDirectoryName(_proc.MainModule.FileName), "logs\\Client.txt");

        public bool IsValid => _proc != null && !_proc.HasExited;

        public LogListener Listener;

        public Game(int pid = 0)
        {
            if (pid == 0)
            {
                var list = GetRunningInstances();

                //TODO
            }
            else
            {
                _proc = Process.GetProcessById(pid);
            }

            _proc.Exited += proc_Exited;
            _proc.EnableRaisingEvents = true;

            Listener = new LogListener(LogFile);

            Listener.AddListener("] :\\s(.+?) \\((.+)\\) is now level ([0-9]+)", OnLevelUp);
            Listener.AddListener("] : You have entered (.+).", OnInstanceEntered);
            Listener.AddListener("] Connecting to instance server at (.+)", OnInstanceConnect);
            Listener.AddListener("] Connected to (.+) in (.+)ms.", OnLogin);

            Listener.StartListening();

            Logger.Get.Info("Added listeners");

            var discordEvents = new DiscordRpc.EventHandlers();
            discordEvents.ReadyCallback += () => Logger.Get.Success("Connected to Discord RPC pipe");
            discordEvents.ErrorCallback += (errCode, msg) => Logger.Get.Error($"Discord error triggered '{errCode}' {msg}");
            discordEvents.DisconnectedCallback += (_, __) => Logger.Get.Info("Disconnected from Discord RPC pipe");

            DiscordRpc.Discord_Initialize("550890770056347648", ref discordEvents, false, null);
        }

        /*
                public static Game Get
                {
                    get
                    {
                        lock (_padlock)
                            return _instance ?? (_instance = new Game());
                    }
                }
        */

        public static IEnumerable<Process> GetRunningInstances()
        {
            return Process.GetProcesses()
                .Where(x => _executableNames.Contains(x.ProcessName));
        }

        internal bool CloseTcpConnections(IpVersion ipVersion = IpVersion.Ipv4)
        {
            if (!IsValid)
                return false;

            Logger.Get.Debug($"Trying to close tcp connections for pid {_proc.Id}");

            var connections = TcpHelper.GetTcpConnections(ipVersion, TcpTableClass.OwnerPidAll);

            if (connections.Count == 0)
                return false;

            foreach (var connection in connections.Where(x => x.OwningPid == _proc.Id))
            {
                if (!TcpHelper.CloseConnection(connection))
                {
                    // TODO
                    Logger.Get.Warning($"Could not kill tcp connection");
                }
            }

            return true;
        }

        public void SendWhisper(string recipientName, string message)
        {
            SendChatMessage($"@{recipientName} {message}");
        }

        public void SendChatMessage(string message)
        {
            throw new NotImplementedException();
        }

        private void proc_Exited(object sender, EventArgs e)
        {
            GameProcessExited?.Invoke(this, e);
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

            _characterInfo = PoeApi.GetCharacterData();

            if (_characterInfo != null)
            {
                _richPresence.Details = $"{_characterInfo.League} League";
                _richPresence.LargeImageKey = _characterInfo.Class.ToLower();
                _richPresence.LargeImageText = $"{_characterInfo.Class} ({_characterInfo.Level})";
            }
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
                _proc?.Dispose();
                Listener?.Dispose();
            }

            DiscordRpc.Discord_Shutdown();

            _disposed = true;
        }
    }
}