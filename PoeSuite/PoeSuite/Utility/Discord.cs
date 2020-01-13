using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Utility
{
    public static class Discord
    {
        [Serializable]
        public struct RichPresence
        {
            public string State;
            public string Details;
            public long StartTimestamp;
            public long EndTimestamp;
            public string LargeImageKey;
            public string LargeImageText;
            public string SmallImageKey;
            public string SmallImageText;
            public string PartyId;
            public int PartySize;
            public int PartyMax;
            public string MatchSecret;
            public string JoinSecret;
            public string SpectateSecret;
            public bool Instance;

            public void Update()
            {
                Discord_UpdatePresence(ref this);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadyCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisconnectedCallback(int errorCode, string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ErrorCallback(int errorCode, string message);

        public struct EventHandlers
        {
            public ReadyCallback ReadyCallback;
            public DisconnectedCallback DisconnectedCallback;
            public ErrorCallback ErrorCallback;
        }

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_Initialize(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_UpdatePresence(ref RichPresence presence);

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_RunCallbacks();

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_Shutdown();
    }
}
