using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace PoeSuite.Imports
{
    internal static class Kernel32
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);
    }
}
