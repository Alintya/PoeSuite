using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Imports
{
    internal static class User32
    {
        private const string libName = "user32.dll";

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x08000000;


        [DllImport(libName)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport(libName)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport(libName)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(libName)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    }
}
