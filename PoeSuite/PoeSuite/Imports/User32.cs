using System.Runtime.InteropServices;
using System.Text;
using System;
using PoeSuite.Utilities;

namespace PoeSuite.Imports
{
    internal static class User32
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x08000000;

        public const uint WINEVENT_OUTOFCONTEXT = 0;
        public const uint EVENT_SYSTEM_FOREGROUND = 3;

        public delegate void WinEventDelegate(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime
        );

        [DllImport("User32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(
            IntPtr hWnd
        );

        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowLong(
            IntPtr hWnd,
            int nIndex,
            int dwNewLong
        );

        [DllImport("User32.dll")]
        public static extern int GetWindowLong(
            IntPtr hWnd,
            int nIndex
        );

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(
            IntPtr hWnd
        );

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(
            IntPtr hWnd,
            StringBuilder text,
            int count
        );

        [DllImport("User32.dll")]
        public static extern IntPtr SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags
        );

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public static string GetActiveWindowTitle()
        {
            var hWnd = GetForegroundWindow();

            

            var titleLen = GetWindowTextLength(hWnd);
            var strBuff = new StringBuilder(titleLen + 1);
            if (GetWindowText(hWnd, strBuff, strBuff.Capacity) > 0)
                return strBuff.ToString();
            else
            {
                return Marshal.GetLastWin32Error().ToString();
            }

            return null;
        }
    }
}
