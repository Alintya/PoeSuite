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
    public class Game
    {
        private static readonly string[] _executableNames =
        {
            "PathOfExile.exe",
            "PathOfExile_x64.exe",
            "PathOfExileSteam.exe",
            "PathOfExile_x64Steam.exe",
            "PathOfExile_KG.exe",
            "PathOfExile_x64_KG.exe"
        };

        public Game()
        {

        }

        public static IEnumerable<Process> GetRunningInstances()
        {
            return Process.GetProcesses()
                .Where(x => _executableNames.Contains(x.ProcessName));
        }

        public static bool CloseTcpConnections(Process proc, IpVersion ipVersion)
        {
            var connections = TcpHelper.GetTcpConnections(ipVersion, TcpTableClass.OwnerPidAll);
            if (connections.Count == 0)
                return false;

            foreach(var connection in connections)
            {
                if (!TcpHelper.CloseConnection(connection))
                {
                    // TODO
                }
            }

            return true;
        }
    }
}
