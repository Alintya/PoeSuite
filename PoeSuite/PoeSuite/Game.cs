﻿using PoeSuite.Imports;
using PoeSuite.Models;
using PoeSuite.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
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

        private bool _disposed;

        private Process _proc;
        private Discord _discord;
        private PoeCharacterInfo _characterInfo;

        public event EventHandler GameProcessExited;

        public string LogFile => Path.Combine(Path.GetDirectoryName(_proc.MainModule.FileName), "logs\\Client.txt");

        public bool IsValid => _proc != null && !_proc.HasExited;

        public bool IsForegroundWindow => User32.GetForegroundWindow() == _proc.MainWindowHandle;

        //public IntPtr WindowHandle => _proc.MainWindowHandle;

        public LogListener Listener;
     

        public Game(Process proc)
        {
            _proc = proc;
            Initialize();
        }

        public Game(int pid)
        {
            _proc = Process.GetProcessById(pid);
            Initialize();
        }

        private void Initialize()
        {
            _proc.Exited += proc_Exited;
            _proc.EnableRaisingEvents = true;

            Listener = new LogListener(LogFile);

            Listener.AddListener("] :\\s(.+?) \\((.+)\\) is now level ([0-9]+)", OnLevelUp);
            Listener.AddListener("] : You have entered (.+).", OnInstanceEntered);
            Listener.AddListener("] Connecting to instance server at (.+)", OnInstanceConnect);
            Listener.AddListener("] Connected to (.+) in (.+)ms.", OnLogin);
            Listener.AddListener("] (?:@From|@To|#|\\$|&|%) ?(.+): (.+)", OnChatMessage);
            //Listener.AddListener("] (?:@From|@To) (.+): (.+)", OnWhisperMessage);

            _discord = new Discord();

            Listener.StartListening();
            Logger.Get.Info("Added game log listeners");

            RegisterHotkeys();

            Logger.Get.Success("Initialized game instance " + _proc.Id);
        }

        public static Game Launch(string filepath)
        {
            var proc = new Process();

            proc.StartInfo.FileName = filepath;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(Properties.Settings.Default.PoeFilePath);
            proc.StartInfo.UseShellExecute = false;

            try
            {
                proc.Start();
            }
            catch (Exception e)
            {
                Logger.Get.Error($"Could not start poe: {e.Message}");
            }
            Logger.Get.Success("Launched PoE");

            return new Game(proc);
        }

        internal static bool ValidateGamePath(string poeFilePath)
        {
            return File.Exists(poeFilePath) && _executableNames.Any(x => poeFilePath.Contains(x));
        }

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
            {
                Logger.Get.Warning($"There are no active tcp connections for {_proc.Id}");
                return false;
            }
                

            foreach (var connection in connections.Where(x => x.OwningPid == _proc.Id))
            {
                if (!TcpHelper.CloseConnection(connection))
                {
                    // TODO
                    Logger.Get.Warning($"Could not kill tcp connection");
                    return false;
                }
            }

            return true;
        }

        public void DisplayWhoIs(string player) => SendChatMessage("/whois " + player);

        public void EnterHideout(string player) => SendChatMessage("/hideout " + player);

        public void KickPlayer(string player) => SendChatMessage("/kick " + player);

        public void LeaveParty() => KickPlayer(_characterInfo.Name);

        public void TradeWith(string player) => SendChatMessage("/tradewith " + player);

        public void InvitePlayer(string player) => SendChatMessage("/invite " + player);

        public void SendWhisper(string recipientName, string message) => SendChatMessage($"@{recipientName} {message}");

        public void ChatLogout() => SendChatMessage("/exit");

        private void SendChatMessage(string message)
        {
            if (!IsForegroundWindow)
                return;

            try
            {
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                System.Windows.Forms.SendKeys.SendWait(message);
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            }
            catch(Exception e)
            {
                Logger.Get.Error($"Could not send chatmessage: {e.Message}");
            }

        }

        private void RegisterHotkeys()
        {
            HotkeysManager.Get.AddCallback("EnterHideout", () => SendChatMessage("/hideout"));
            //HotkeysManager.Get.AddCallback("EnterDelve", () => SendChatMessage("/delve"));
            //HotkeysManager.Get.AddCallback("EnterMenagerie", () => SendChatMessage("/menagerie"));
            //HotkeysManager.Get.AddCallback("EnterHideout", () => SendChatMessage("/remaining"));
        }

        private void UnregisterHotkeys()
        {
            HotkeysManager.Get.ClearCallbacks("EnterHideout");
        }

        /// <summary>
        /// Gets triggered when the player receives a chat message
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private static void OnChatMessage(string line, Match match)
        {
            // TODO: filter message channel?
            Logger.Get.Debug($"Chat message from: {match.Groups[1]}: {match.Groups[2]}");
        }

        /// <summary>
        /// Gets triggered when the player receives a private message
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private static void OnWhisperReceived(string line, Match match)
        {
            Logger.Get.Debug($"Connected to server instance {match.Groups[1]}");
        }

        /// <summary>
        /// Gets triggered when the player connects to a new instance
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private void OnInstanceConnect(string line, Match match)
        {
            Logger.Get.Debug($"Connected to server instance {match.Groups[1]}");

            _characterInfo = PoeApi.GetCharacterData();

            if (_characterInfo is null)
                return;

            _discord.RichPresenceData.Details = $"{_characterInfo.League} League";
            _discord.RichPresenceData.LargeImageKey = _characterInfo.Class.ToLower();
            _discord.RichPresenceData.LargeImageText = $"{_characterInfo.Class} ({_characterInfo.Level})";
        }

        /// <summary>
        /// Gets triggered when the player enters a new instance
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private void OnInstanceEntered(string line, Match match)
        {
            var location = match.Groups[1].Value;

            Logger.Get.Debug($"Traveled to {location}");

            _discord.RichPresenceData.State = location;
            _discord.RichPresenceData.StartTimestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            _discord.RichPresenceData.SendUpdate();
        }

        /// <summary>
        /// Gets triggered when the player levels up
        /// </summary>
        /// <param name="line"></param>
        /// <param name="match"></param>
        private void OnLevelUp(string line, Match match)
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

            _discord.RichPresenceData.LargeImageText = $"{charClass} ({charLevel})";
            _discord.RichPresenceData.SendUpdate();
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

        private void proc_Exited(object sender, EventArgs e)
        {
            GameProcessExited?.Invoke(this, e);
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
                UnregisterHotkeys();

                _proc?.Dispose();
                Listener?.Dispose();
                _discord?.Dispose();
            }
            _disposed = true;
        }
    }
}