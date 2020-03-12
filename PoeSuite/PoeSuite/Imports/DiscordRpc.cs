using System.Runtime.InteropServices;
using System;

namespace PoeSuite.Imports
{
    internal static class DiscordRpc
    {
        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_RunCallbacks();

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_Shutdown();

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void Discord_Initialize(
            string applicationId,
            ref EventHandlers handlers,
            bool autoRegister,
            string optionalSteamId
        );

        [DllImport("discord-rpc.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Discord_UpdatePresence(
            ref RichPresence presence
        );

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

            public void SendUpdate()
            {
                //Discord_RunCallbacks();

                if (Properties.Settings.Default.DiscordRichPresence)
                    Discord_UpdatePresence(ref this);
            }
        }
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
