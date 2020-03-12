using System.Runtime.InteropServices;
using System;

namespace PoeSuite.Imports
{
    internal static class Iphlpapi
    {
        public enum IpVersion
        {
            Ipv4 = 2,
            Ipv6 = 23
        }

        public enum TcpTableClass
        {
            BasicListener,
            BasicConnetions,
            BasicAll,
            OwnerPidListener,
            OwnerPidConnections,
            OwnerPidAll,
            OwnerModuleListener,
            OwnerModuleConnections,
            OwnerModuleAll
        }

        public enum MibTcpState
        {
            Closed = 1,
            Listen = 2,
            SynchronizeSent = 3,
            SynchronizeReceived = 4,
            Established = 5,
            FinWait1 = 6,
            FinWait2 = 7,
            CloseWait = 8,
            Closing = 9,
            LastAck = 10,
            TimeWait = 11,
            DeleteTcp = 12
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MibTcpTableOwnerPid
        {
            public uint TableCount;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
            public MibTcpRowOwnerId[] Tables;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MibTcpRowOwnerId
        {
            public MibTcpState State;
            public uint LocalAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] LocalPort;
            public uint RemoteAddr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] RemotePort;
            public uint OwningPid;
        }

        [DllImport("Iphlpapi.dll", SetLastError = true)]
        public static extern uint GetExtendedTcpTable(
            IntPtr tcpTable,
            ref int outBufLen,
            bool sort,
            IpVersion ipVersion,
            TcpTableClass tableClass,
            uint reserved = 0);

        [DllImport("Iphlpapi.dll", SetLastError = true)]
        public static extern int SetTcpEntry(
            IntPtr tcpRow
        );
    }
}
