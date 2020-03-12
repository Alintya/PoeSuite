using System;

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
