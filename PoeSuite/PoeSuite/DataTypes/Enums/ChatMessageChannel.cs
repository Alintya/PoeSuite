using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.DataTypes.Enums
{
    [Flags]
    public enum ChatMessageChannel
    {
        None,
        System,
        Local,
        Private,
        Global,
        Trade,
        Party,
        Guild,
        ChatCommand,
        Unknown
    }
}
