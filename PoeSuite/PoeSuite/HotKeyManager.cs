using PoeSuite.Imports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoeSuite
{
    // TODO: add callback invocation
    class HotKeyManager : NativeWindow
    {
        [Flags]
        public enum FsModifier
        {
            None = 0x0000,
            Alt = 0x0001,           // Either ALT key must be held down
            Control = 0x0002,       // Either CTRL key must be held down
            NoRepeat = 0x4000,      // Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.
            Shift = 0x0004,         // Either SHIFT key must be held down
            Win = 0x0008,           // Either WINDOWS key must be held down
        }

        private const int WM_HOTKEY = 0x0312;
        private const int WM_DESTROY = 0x0002;


        private List<int> _ids = new List<int>();
        private int _idCounter = 0;

        public delegate void HotkeyDelegate(int id);
        public event HotkeyDelegate HotkeyPressed;

        public HotKeyManager()
        {
            this.CreateHandle(new CreateParams());
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            RegisterCombo(1001, 1, (int)Keys.D1);
            RegisterCombo(1002, 1, (int)Keys.D2);
            RegisterCombo(1003, 1, (int)Keys.Q);
        }

        public void RegisterHotkey(Keys key, FsModifier modifier = FsModifier.None)
        {
            if (User32.RegisterHotKey(this.Handle, _idCounter++, (int)modifier, (int)key))
            {
                _ids.Add(_idCounter);
            }
        }

        public void RegisterHotkey(Keys key, Action callback, FsModifier modifier = FsModifier.None)
        {
            if (User32.RegisterHotKey(this.Handle, _idCounter++, (int)modifier, (int)key))
            {
                _ids.Add(_idCounter);
            }
        }

        public void RegisterCombo(Int32 id, int fsModifiers, int vlc)
        {
            if (User32.RegisterHotKey(this.Handle, id, fsModifiers, vlc))
            {
                _ids.Add(id);
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
                    HotkeyPressed?.Invoke(m.WParam.ToInt32());


                    break;

                case WM_DESTROY: // fires when "Application.Exit();" is called
                    foreach (int id in _ids)
                    {
                        User32.UnregisterHotKey(this.Handle, id);
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
