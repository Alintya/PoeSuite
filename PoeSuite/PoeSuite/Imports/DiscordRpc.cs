using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Imports
{
    internal class DiscordRpc
    {
        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void Discord_Initialize(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_UpdatePresence(ref Models.RichPresence presence);

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_RunCallbacks();

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_Shutdown();

        public struct EventHandlers
        {
            public ReadyCallback ReadyCallback;
            public DisconnectedCallback DisconnectedCallback;
            public ErrorCallback ErrorCallback;
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadyCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisconnectedCallback(int errorCode, string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ErrorCallback(int errorCode, string message);
    }
}
