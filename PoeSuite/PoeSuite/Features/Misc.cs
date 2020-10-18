using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LowLevelInput.Hooks;
using PoeSuite.Models;
using PoeSuite.Utilities;

namespace PoeSuite.Features
{
    internal partial class Misc
    {
        private readonly GameSettings _settings;
        private Point _lastMousePos = new Point(0, 0);
        public Misc(GameSettings settings)
        {
            _settings = settings;

            HotkeysManager.Get.AddCallback("MouseScroll", ExtendTabScrolling);
            HotkeysManager.Get.AddModifier("MouseScroll", VirtualKeyCode.Lcontrol);
            HotkeysManager.Get.AddCallback("MouseMove", ExtendTabScrolling);
            HotkeysManager.Get.AddModifier("MouseMove", VirtualKeyCode.Lcontrol);
        }

        private void ExtendTabScrolling(VirtualKeyCode key, int x, int y)
        {
            if (key == VirtualKeyCode.Scroll)
            {
                var sidebar = _settings.GetSideBarWidth;
                var dir = (short) x;

                if (_lastMousePos.X > sidebar)
                {
                    System.Windows.Forms.SendKeys.SendWait(dir > 0 ? "{LEFT}" : "{RIGHT}");
                }
            }
            else
            {
                _lastMousePos = new Point(x, y);
            }

            //Logger.Get.Debug();
        }
    }

    internal partial class Misc
    {

    }
}
