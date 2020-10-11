using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System;

namespace PoeSuite.Imports
{
    internal static class Kernel32
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(
            IntPtr handle
        );

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(int dwProcessId);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();

        [DllImport("Kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("Kernel32.dll")]
        public static extern bool SetConsoleTitleA(string lpConsoleTitle);

        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
    }
}

