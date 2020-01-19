using PoeSuite.Imports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Models
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
            DiscordRpc.Discord_UpdatePresence(ref this);
        }
    }
}
