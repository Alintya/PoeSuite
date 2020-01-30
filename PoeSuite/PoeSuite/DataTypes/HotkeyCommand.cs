using LowLevelInput.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.DataTypes
{
    internal class HotkeyCommand
    {
        public VirtualKeyCode KeyCode;
        public VirtualKeyCode Modifier = VirtualKeyCode.Invalid;
        public KeyState State = KeyState.Up;
        public List<Action> Actions = new List<Action>();
    }
}
