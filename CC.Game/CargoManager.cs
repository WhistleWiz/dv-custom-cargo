using CC.Common;
using DV.ThingTypes;
using DVLangHelper.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    newCargos.Add(customCargo);
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

            // Handle duplicate names (not).
            if (DV.Globals.G.Types.cargos.Any(x => x.id == c.Name))
            {
                CCMod.Error($"Cargo with name '{c.Name}' already exists!");
                return null;
            }

            // Assign a new enum value, and increment the counter so the next one isn't the same.
            c.Id = s_cargoV1++;

            // Convert into actual cargo and add it.
            CargoType_v2 v2 = CargoInjector.ToV2(c);
            DV.Globals.G.Types.cargos.Add(v2);

            // Cache the new enum so it can be patched in.
            AddedValues.Add(v2.v1);

            // Add translations for this cargo.
            AddTranslations(c);

            return v2;
        }

        private static void AddTranslations(CustomCargo cargo)
        {
            if (!string.IsNullOrEmpty(cargo.Csv))
            {
                CCMod.Translations.AddTranslationsFromWebCsv(cargo.Csv!);
                return;
            }

            // If there are no translations, use the name as default.
            if (cargo.TranslationDataFull == null)
            {
                CCMod.Translations.AddTranslations(
                    cargo.LocalizationKeyFull,
                    TranslationData.Default(cargo.Name));
            }
            else
            {
                CCMod.Translations.AddTranslations(
                    cargo.LocalizationKeyFull,
                    cargo.TranslationDataFull);
            }

            if (cargo.TranslationDataShort == null)
            {
                CCMod.Translations.AddTranslations(
                    cargo.LocalizationKeyShort,
                    TranslationData.Default(cargo.Name));
            }
            else
            {
                CCMod.Translations.AddTranslations(
                    cargo.LocalizationKeyShort,
                    cargo.TranslationDataShort);
            }
        }
    }
}
