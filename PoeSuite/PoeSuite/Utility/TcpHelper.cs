using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static PoeSuite.Imports.Iphlpapi;

namespace PoeSuite.Utility
{
    internal static class TcpHelper
    {
        public static bool CloseConnection(MibTcpRowOwnerId tcpRow)
        {
            tcpRow.State = MibTcpState.Closed;

            var tableSize = Marshal.SizeOf(tcpRow);
            var tcpRowPtr = Marshal.AllocCoTaskMem(tableSize);

            Marshal.StructureToPtr(tcpRow, tcpRowPtr, false);

            return SetTcpEntry(tcpRowPtr) == 0;
        }

        public static List<MibTcpRowOwnerId> GetTcpConnections(IpVersion ipVersion, TcpTableClass tableClass)
        {
            int tableSize = 0;
            GetExtendedTcpTable(IntPtr.Zero, ref tableSize, true, ipVersion, tableClass);

            var tcpTablePtr = Marshal.AllocHGlobal(tableSize);
            var tcpConnections = new List<MibTcpRowOwnerId>();

            try
            {
                if (GetExtendedTcpTable(tcpTablePtr, ref tableSize, true, ipVersion, tableClass) != 0)
                    return tcpConnections;

                var table = Marshal.PtrToStructure<MibTcpTableOwnerPid>(tcpTablePtr);
                var rowPtr = IntPtr.Add(tcpTablePtr, Marshal.SizeOf(table.EntryCount));
                for (var i = 0; i < table.EntryCount; i++)
                {
                    var tcpRow = Marshal.PtrToStructure<MibTcpRowOwnerId>(rowPtr);

                    tcpConnections.Add(tcpRow);

                    rowPtr = IntPtr.Add(rowPtr, Marshal.SizeOf(tcpRow));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }

            return tcpConnections;
        }
    }
}
