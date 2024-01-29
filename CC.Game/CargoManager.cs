using CC.Common;
using CC.Game;
using DV;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using DVLangHelper.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace CC.Game
{
    internal static class CargoManager
    {
        // Custom v1 values start at 10k.
        private const int Start = 10000;

        // Global counter to never reuse the same value.
        private static int s_cargoV1 = Start;

        public static HashSet<CargoType> AddedValues = new HashSet<CargoType>();
        public static List<CustomCargo> AddedCargos = new List<CustomCargo>();

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

                if (TryLoadCargo(jsonPath, json, out var customCargo))
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

                Globals.G.Types.RecalculateCaches();
            }
        }

        private static bool TryLoadCargo(string jsonPath, JObject json, out CargoType_v2 v2)
        {
            CustomCargo? c = json.ToObject<CustomCargo>();

            if (c == null)
            {
                CCMod.Error($"Could not load cargo from file '{jsonPath}'");
                v2 = null!;
                return false;
            }

            // Handle duplicate names (not).
            if (Globals.G.Types.cargos.Any(x => x.id == c.Name))
            {
                CCMod.Error($"Cargo with name '{c.Name}' already exists!");
                v2 = null!;
                return false;
            }

            // Assign a new enum value, and increment the counter so the next one isn't the same.
            c.Id = s_cargoV1++;

            if (c.CargoGroups.Length == 0)
            {
                c.CargoGroups = new[] { c.GetDefaultCargoGroup() };
            }

            // Convert into actual cargo and add it.
            v2 = CargoManager.ToV2(c);
            Globals.G.Types.cargos.Add(v2);

            // Cache the new enum so it can be patched in,
            // and the cargo for easy access.
            AddedValues.Add(v2.v1);
            AddedCargos.Add(c);

            // Add translations for this cargo.
            AddTranslations(c);

            return true;
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

        public static List<CargoType> ToCargoTypeGroup(CustomCargoGroup group, string cargoName)
        {
            List<CargoType> types = new List<CargoType>();

            foreach (var item in group.CargosIds)
            {
                if (!Globals.G.Types.cargos.TryFind(x => x.id == item, out var cargo))
                {
                    CCMod.Error($"Cargo '{item}' is missing (cargo group for '{cargoName}')");
                    continue;
                }

                types.Add(cargo.v1);
            }

            return types;
        }

        public static CargoType_v2 ToV2(this CustomCargo cargo)
        {
            var newCargo = ScriptableObject.CreateInstance<CargoType_v2>();

            newCargo.id = cargo.Name;
            newCargo.v1 = (CargoType)cargo.Id;

            newCargo.localizationKeyFull = cargo.LocalizationKeyFull;
            newCargo.localizationKeyShort = cargo.LocalizationKeyShort;

            newCargo.massPerUnit = cargo.MassPerUnit;
            newCargo.fullDamagePrice = cargo.FullDamagePrice;
            newCargo.environmentDamagePrice = cargo.EnvironmentDamagePrice;

            newCargo.requiredJobLicenses = cargo.Licenses.Select(x => ((JobLicenses)x).ToV2()).ToArray();
            newCargo.loadableCarTypes = Array.Empty<CargoType_v2.LoadableInfo>();

            return newCargo;
        }
    }
}
