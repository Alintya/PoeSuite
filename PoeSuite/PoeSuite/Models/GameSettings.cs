using System;
using System.Runtime.InteropServices;
using PoeSuite.Imports;

namespace PoeSuite.Models
{
    public class GameSettings
    {
        private const float SideBarRatio = 370f / 600;

        public int GetSideBarWidth => (int)(ResolutionHeight * SideBarRatio);

        public bool Fullscreen;
        public bool BorderlessWindow;
        public int ResolutionWidth;
        public int ResolutionHeight;
        public char ChatKey;

        public void Update(IntPtr hWnd)
        {
            User32.RECT rect;

            User32.GetWindowRect(hWnd, out rect);

            ResolutionHeight = rect.Bottom - rect.Top;
            ResolutionWidth = rect.Right - rect.Left;

            // TODO
        }
    }
}
