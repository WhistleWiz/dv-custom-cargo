using CC.Common;
using DV.ThingTypes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityModManagerNet;

namespace CC.Game
{
    internal class CargoManager
    {
        // Custom v1 values start at 10k.
        private const int Start = 10000;

        // Global counter to never reuse the same value.
        private static int s_cargoV1 = Start;

        public static HashSet<CargoType> AddedValues = new HashSet<CargoType>();

        public static void LoadCargos(UnityModManager.ModEntry mod)
        {
            List<CargoType_v2> newCargos = new List<CargoType_v2>();

            foreach (string jsonPath in Directory.EnumerateFiles(mod.Path, NameConstants.CargoFile, SearchOption.AllDirectories))
            {
                JObject json;

                try
                {
                    using (StreamReader reader = File.OpenText(jsonPath))
                    {
                        json = JObject.Parse(reader.ReadToEnd());
                    }
                }
                catch (Exception ex)
                {
                    CCMod.Error($"Error loading file {jsonPath}:\n{ex.Message}");
                    continue;
                }

                var customCargo = LoadCargo(jsonPath, json);

                if (customCargo != null)
                {
                    AddedValues.Add(customCargo.v1);
                    newCargos.Add(customCargo);

                    CCMod.Translations.AddTranslation(
                        customCargo.localizationKeyFull,
                        DVLangHelper.Data.DVLanguage.English,
                        customCargo.id);
                    CCMod.Translations.AddTranslation(
                        customCargo.localizationKeyShort,
                        DVLangHelper.Data.DVLanguage.English,
                        customCargo.id);
                }
            }

            if (newCargos.Count > 0)
            {
                CCMod.Log($"Loaded {newCargos.Count} cargos from {mod.Path}");

                foreach (var item in newCargos)
                {
                    CCMod.Log($"{item.v1} | {item.id}");
                }

                DV.Globals.G.Types.RecalculateCaches();
            }
        }

        private static CargoType_v2? LoadCargo(string jsonPath, JObject json)
        {
            CustomCargo? c = json.ToObject<CustomCargo>();

            if (c == null)
            {
                CCMod.Error($"Could not load cargo from file '{jsonPath}'");
                return null;
            }

            if (DV.Globals.G.Types.CargoType_to_v2.TryGetValue((CargoType)c.Id, out CargoType_v2 existingCargo))
            {
                CCMod.Error($"Cargo {c.Name} (ID: {c.Id}) already exists ({existingCargo.id})!");
                return null;
            }

            // Assign a new enum value, and increment the counter so the next one isn't the same.
            c.Id = s_cargoV1++;
            CargoType_v2 v2 = CargoInjector.ToV2(c);

            DV.Globals.G.Types.cargos.Add(v2);

            return v2;
        }
    }
}
