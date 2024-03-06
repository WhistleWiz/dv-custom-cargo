using CC.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CC.Game
{
    internal class SaveInjector
    {
        private const string s_keys = "Keys";
        private const string s_values = "Values";

        internal static JObject? LoadedData;

        internal static void ExtractDataFromSaveGame(SaveGameData data)
        {
            StateManager.RequestUnload();
            LoadedData = data.GetJObject(Constants.SaveKey);

            if (LoadedData != null)
            {
                // Really...
                var keys = LoadedData[s_keys]!.ToObject<string[]>();
                var values = LoadedData[s_values]!.ToObject<int[]>();

                if (keys != null && values != null)
                {
                    var dictionary = new Dictionary<string, int>();

                    for (int i = 0; i < keys.Length; i++)
                    {
                        dictionary.Add(keys[i], values[i]);
                    }

                    CargoManager.Mapping = dictionary;
                    CCMod.Log("Mapping cache sucessfully loaded.");
                }
                else
                {
                    CCMod.Error("Error loading data: mapping is null!");
                }
            }
            else
            {
                CCMod.Warning("No data found in save file, using new cache.");
            }
        }

        internal static void InjectDataIntoSaveGame(SaveGameData data)
        {
            // Why can't I just put the dictionary in there...
            LoadedData = new JObject
            {
                { s_keys, JToken.FromObject(CargoManager.Mapping.Keys.ToArray()) },
                { s_values, JToken.FromObject(CargoManager.Mapping.Values.ToArray()) }
            };

            data.SetJObject(Constants.SaveKey, LoadedData);
        }
    }
}
