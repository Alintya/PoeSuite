using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoeSuite
{
    class HotKeyManager : NativeWindow
    {

        public static void OnPreviewKeyUp(object source, KeyEventArgs e)
        {
            Console.WriteLine($"Key pressed: {e.KeyCode}");
        }

        private const int WM_HOTKEY = 0x0312;
        private const int WM_DESTROY = 0x0002;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private List<Int32> IDs = new List<int>();

        public delegate void HotkeyDelegate(int ID);
        public event HotkeyDelegate HotkeyPressed;

        public HotKeyManager()
        {
            this.CreateHandle(new CreateParams());
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            RegisterCombo(1001, 1, (int)Keys.D1);
            RegisterCombo(1002, 1, (int)Keys.D2);
            RegisterCombo(1003, 1, (int)Keys.Q);
        }

        private void RegisterCombo(Int32 ID, int fsModifiers, int vlc)
        {
            if (RegisterHotKey(this.Handle, ID, fsModifiers, vlc))
            {
                IDs.Add(ID);
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            this.DestroyHandle();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    if (HotkeyPressed != null)
                    {
                        HotkeyPressed(m.WParam.ToInt32());
                    }
                    break;

                case WM_DESTROY: // fires when "Application.Exit();" is called
                    foreach (int ID in IDs)
                    {
                        UnregisterHotKey(this.Handle, ID);
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
