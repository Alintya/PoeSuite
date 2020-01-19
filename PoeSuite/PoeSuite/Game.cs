using PoeSuite.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PoeSuite.Imports.Iphlpapi;

namespace PoeSuite
{
    internal class Game
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

        private static readonly object _padlock = new object();
        private static Game _instance = null;

        private Process _proc = default;

        public bool IsValid => _proc != null && !_proc.HasExited;

        public Game()
        {

        }

        public static Game Get
        {
            get
            {
                lock (_padlock)
                    return _instance ?? (_instance = new Game());
            }
        }

        public static IEnumerable<Process> GetRunningInstances()
        {
            return Process.GetProcesses()
                .Where(x => _executableNames.Contains(x.ProcessName));
        }

        public bool CloseTcpConnections(IpVersion ipVersion = IpVersion.Ipv4)
        {
            var connections = TcpHelper.GetTcpConnections(ipVersion, TcpTableClass.OwnerPidAll);
            if (connections.Count == 0)
                return false;

            foreach(var connection in connections.Where(x => x.OwningPid == _proc.Id))
            {
                if (!TcpHelper.CloseConnection(connection))
                {
                    // TODO
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
    }
}
