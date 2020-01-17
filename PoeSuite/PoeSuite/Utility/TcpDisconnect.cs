using PoeSuite.Imports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Utility
{
    internal class TcpDisconnect
    {
        public static void ShowActiveTcpConnections()
        {
            Console.WriteLine("Active TCP Connections");
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation c in connections)
            {
                Console.WriteLine("{0} <==> {1}",
                                  c.LocalEndPoint.ToString(),
                                  c.RemoteEndPoint.ToString());
            }
        }

        private static List<IPR> GetTCPConnections<IPR, IPT>(int ipVersion = IpHelper.AF_INET)  // IPR = Row Type, IPT = Table Type
        {
            IPR[] tableRows;
            int buffSize = 0;

            var dwNumEntriesField = typeof(IPT).GetField("dwNumEntries");

            // how much memory do we need?
            uint ret = IpHelper.GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, ipVersion, IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            IntPtr tcpTablePtr = Marshal.AllocHGlobal(buffSize);

            try
            {
                ret = IpHelper.GetExtendedTcpTable(tcpTablePtr, ref buffSize, true, ipVersion, IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
                if (ret != 0)
                    return new List<IPR>();

                // get the number of entries in the table
                IPT table = (IPT)Marshal.PtrToStructure(tcpTablePtr, typeof(IPT));
                int rowStructSize = Marshal.SizeOf(typeof(IPR));
                uint numEntries = (uint)dwNumEntriesField.GetValue(table);

                // buffer we will be returning
                tableRows = new IPR[numEntries];

                IntPtr rowPtr = (IntPtr)((long)tcpTablePtr + 4);
                for (int i = 0; i < numEntries; i++)
                {
                    IPR tcpRow = (IPR)Marshal.PtrToStructure(rowPtr, typeof(IPR));
                    tableRows[i] = tcpRow;
                    rowPtr = (IntPtr)((long)rowPtr + rowStructSize);   // next entry
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(tcpTablePtr);
            }
            return tableRows != null ? tableRows.ToList() : new List<IPR>();
        }

        public static bool CloseTcpConnection(int procId)
        {
            int tableSize = 0;
            IpHelper.GetExtendedTcpTable(IntPtr.Zero, ref tableSize, true, IpHelper.AF_INET, IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

            var tcpTablePtr = Marshal.AllocHGlobal(tableSize);

            try
            {
                if (IpHelper.GetExtendedTcpTable(tcpTablePtr, ref tableSize, true, IpHelper.AF_INET, IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL) != 0)
                    return false;

                var table = Marshal.PtrToStructure<IpHelper.MIB_TCPTABLE_OWNER_PID>(tcpTablePtr);
                var rowPtr = IntPtr.Add(tcpTablePtr, Marshal.SizeOf(table.dwNumEntries));
                for (var i = 0; i < table.dwNumEntries; i++)
                {
                    var tcpRow = Marshal.PtrToStructure<IpHelper.MIB_TCPROW_OWNER_PID>(rowPtr);
                    if (tcpRow.OwningPid == procId)
                    {
                        tcpRow.State = 12/*DELETE_TCB*/;

                        var tcpRowPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tcpRow));

                        Marshal.StructureToPtr(tcpRow, tcpRowPtr, false);

                        return IpHelper.SetTcpEntry(tcpRowPtr) == 0;
                    }

                    rowPtr = IntPtr.Add(rowPtr, Marshal.SizeOf(tcpRow));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }

            return false;
        }

        public static void CurrPortsDisconnect(string id)
        {
            //"cports.exe /stext TEMP"
            var cports = new Process();

            // Configure the process using the StartInfo properties.
            cports.StartInfo.FileName = "cports.exe";
            cports.StartInfo.Arguments = $"/close * * * * {id}";
            cports.StartInfo.UseShellExecute = true;
            cports.StartInfo.Verb = "runas";
            //cports.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cports.Start();
            cports.WaitForExit();// Waits here for the process to exit.
        }
    }
}
