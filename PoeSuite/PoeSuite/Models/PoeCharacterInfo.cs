using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSuite.Models
{
#pragma warning disable CS0649

    class PoeCharacterInfo
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("league")]
        public string League;

        [JsonProperty("classId")]
        public int ClassId;

        [JsonProperty("ascendancyClass")]
        public int AscendancyClass;

        [JsonProperty("class")]
        public string Class;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("experience")]
        public int Experience;

        [JsonProperty("lastActive")]
        public bool LastActive;
    }
#pragma warning restore CS0649
}
