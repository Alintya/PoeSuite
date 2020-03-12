using PoeSuite.Models;

using Newtonsoft.Json;

using System.Threading.Tasks;
using System.Net;
using System;

namespace PoeSuite.Utilities
{
    internal static class PoeApi
    {
        private static WebClient _webclient = new WebClient();

        public static PoeCharacterInfo GetCharacterData()
        {
            PoeCharacterInfo characterInfo = new PoeCharacterInfo();

            if (string.IsNullOrEmpty(Properties.Settings.Default.AccountName))
            {
                Logger.Get.Error("Account name is not set!");
                return null;
            }

            try
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.SessionId))
                    _webclient.Headers.Add("POESESSID", Properties.Settings.Default.SessionId);

                var rawData = _webclient.DownloadString(
                            "https://www.pathofexile.com/character-window/get-characters?accountName=" + Properties.Settings.Default.AccountName);

                if (rawData.Length == 0)
                {
                    Logger.Get.Error("Data retrived from PoE api was empty");
                    return null;
                }

                characterInfo = Array.Find(JsonConvert.DeserializeObject<PoeCharacterInfo[]>(rawData), x => x.LastActive);
            }
            catch (WebException ex)
            {
                // profile probably private or wrong name/id
                if (ex.Status == WebExceptionStatus.ProtocolError
                    && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Forbidden
                    && string.IsNullOrEmpty(Properties.Settings.Default.SessionId))
                {
                    Logger.Get.Error($"Failed to retrieve data from PoE api, profile is probably private: {ex.Message}");
                    // TODO: notify user
                }
                else
                {
                    Logger.Get.Error($"Failed to retrieve data from PoE api: {ex.Message}");
                }

                return null;
            }

            Logger.Get.Success($"Updated character info for {characterInfo.Name}!");

            return characterInfo;
        }

        public static async Task<string[]> GetActiveTradingLeagues()
        {
            var list = new string[] { };

            var x = await _webclient.DownloadStringTaskAsync("http://api.pathofexile.com/leagues?type=main&compact=1");

            try
            {
                var leagues = JsonConvert.DeserializeObject<LeagueData[]>(x);
            }
            catch
            {

            }

            //list.
            return list;
        }
    }

    class LeagueData
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("realm")]
        public string Realm;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("startAt")]
        public DateTime? StartAt;

        [JsonProperty("endAt")]
        public DateTime? EndAt;

        [JsonProperty("delveEvent")]
        public bool DelveEvent;
    }
}
