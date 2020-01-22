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

        private Process _proc;
        private bool _disposed;
        private Discord _discord;

        public event EventHandler GameProcessExited;
        public event EventHandler GameOn;

        public string LogFile => Path.Combine(Path.GetDirectoryName(_proc.MainModule.FileName), "logs\\Client.txt");

        public bool IsValid => _proc != null && !_proc.HasExited;

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

            Listener.AddListener("] (?:@From|@To|#|\\$|&|%) ?(.+): (.+)", OnChatMessage);
            //Listener.AddListener("] (?:@From|@To) (.+): (.+)", OnWhisperMessage);

            _discord = new Discord(Listener);

            Listener.StartListening();

            Logger.Get.Info("Added game log listeners");
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
                _discord.Dispose();
            }
            _disposed = true;
        }
    }
}