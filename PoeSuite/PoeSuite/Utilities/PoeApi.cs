using Newtonsoft.Json;
using PoeSuite.Models;
using System;
using System.Net;

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
                var rawData = _webclient.DownloadString(
                            "https://www.pathofexile.com/character-window/get-characters?accountName=" + Properties.Settings.Default.AccountName);

                if (rawData.Length == 0)
                {
                    Logger.Get.Error("Data retrived from PoE api was empty");
                    return null;
                }
                    
                characterInfo = Array.Find(JsonConvert.DeserializeObject<PoeCharacterInfo[]>(rawData), x => x.LastActive);
            }
            catch (Exception ex)
            {
                Logger.Get.Error($"Failed to retrieve data from PoE api: {ex}");
                return null;
            }

            return characterInfo;
        }
    }
}
