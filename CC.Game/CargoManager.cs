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
        public static HashSet<CargoType> AddedValues = new HashSet<CargoType>();
        public static List<CustomCargo> AddedCargos = new List<CustomCargo>();

        public static void LoadCargos(UnityModManager.ModEntry mod)
        {
            List<CargoType_v2> newCargos = new List<CargoType_v2>();

            // Find the 'cargo.json' files.
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

                // Try to load the cargo if we have one of those files.
                if (TryLoadCargo(jsonPath, json, out var customCargo))
                {
                    newCargos.Add(customCargo);
                }
            }

            // If we did load any cargo...
            if (newCargos.Count > 0)
            {
                CCMod.Log($"Loaded {newCargos.Count} cargos from {mod.Path}");

                foreach (var item in newCargos)
                {
                    CCMod.Log($"{item.v1} | {item.id}");
                }

                // Recalculate cache so the game knows we added a new cargo.
                Globals.G.Types.RecalculateCaches();
            }
        }

        private static bool TryLoadCargo(string jsonPath, JObject json, out CargoType_v2 v2)
        {
            CustomCargo? c = json.ToObject<CustomCargo>();
            var directory = Path.GetDirectoryName(jsonPath);

            // Something is wrong with the file.
            if (c == null)
            {
                CCMod.Error($"Could not load cargo from file '{jsonPath}'");
                v2 = null!;
                return false;
            }

            // Handle duplicate names (not).
            if (Globals.G.Types.cargos.Any(x => x.id == c.Identifier))
            {
                CCMod.Error($"Cargo with name '{c.Identifier}' already exists!");
                v2 = null!;
                return false;
            }

            // If no cargo groups were defined, assume a cargo group with only this cargo.
            if (c.CargoGroups.Length == 0)
            {
                c.CargoGroups = new[] { c.GetDefaultCargoGroup() };
            }
            else
            {
                // Ensure the cargo that defines the groups is in them.
                foreach (var group in c.CargoGroups)
                {
                    group.AddIdIfMissing(c.Identifier);
                }
            }

            // Convert into actual cargo and add it.
            v2 = c.ToV2();
            Globals.G.Types.cargos.Add(v2);

            // Cache the new enum so it can be patched in,
            // and the cargo for easy access.
            AddedValues.Add(v2.v1);
            AddedCargos.Add(c);

            // Add translations for this cargo.
            AddTranslations(c);

            // Try to load icon files.
            TryLoadIcons(directory, out var icon, out var resourceIcon);

            if (icon != null)
            {
                v2.icon = icon;
            }

            if (resourceIcon != null)
            {
                v2.resourceIcon = resourceIcon;
            }

            // Try to load any asset bundle with models.
            TryLoadModels(directory, out var models);

            // Add vanilla types to loadable info.
            AddLoadableInfo(c, v2, models);

            return true;
        }

        private static bool TryLoadModels(string directory, out ModelsForVanillaCar[] models)
        {
            var assetBundlePath = Path.Combine(directory, NameConstants.ModelBundle);

            if (!File.Exists(assetBundlePath))
            {
                CCMod.Log($"No model bundle found (expected {assetBundlePath}).");
                models = null!;
                return false;
            }

            CCMod.Log($"Loading model bundle...");
            var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

            if (assetBundle == null)
            {
                CCMod.Error($"Failed to load model bundle!");
                models = null!;
                return false;
            }

            models = assetBundle.LoadAllAssets<ModelsForVanillaCar>();

            if (models.Length == 0)
            {
                CCMod.Error($"No models found in the bundle!");
            }

            foreach (var item in models)
            {
                foreach (var prefab in item.Prefabs)
                {
                    // Ask CCL to handle some model loading here, so we can support
                    // the proxy system on custom cargo.
                }
            }

            assetBundle.Unload(false);

            return true;
        }

        private static void TryLoadIcons(string directory, out Sprite? icon, out Sprite? resourceIcon)
        {
            byte[] data;

            var path = Path.Combine(directory, NameConstants.Icon);

            if (File.Exists(path))
            {
                data = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                icon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
            }
            else
            {
                icon = null;
            }

            path = Path.Combine(directory, NameConstants.ResourceIcon);

            if (File.Exists(path))
            {
                data = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                resourceIcon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
            }
            else
            {
                resourceIcon = null;
            }
        }

        private static void AddTranslations(CustomCargo cargo)
        {
            if (!string.IsNullOrEmpty(cargo.CSVLink))
            {
                CCMod.Translations.AddTranslationsFromWebCsv(cargo.CSVLink!);
                return;
            }

            // If there are no translations, use the name as default.
            if (cargo.TranslationDataFull == null)
            {
                CCMod.Translations.AddTranslations(
                    cargo.LocalizationKeyFull,
                    TranslationData.Default(cargo.Identifier));
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
                    TranslationData.Default(cargo.Identifier));
            }
            else
            {
                CCMod.Translations.AddTranslations(
                    cargo.LocalizationKeyShort,
                    cargo.TranslationDataShort);
            }
        }

        private static void AddLoadableInfo(CustomCargo cargo, CargoType_v2 v2, ModelsForVanillaCar[] models)
        {
            List<CargoType_v2.LoadableInfo> loadables = v2.loadableCarTypes.ToList();

            foreach (var item in cargo.VanillaTypesToLoad)
            {
                GameObject[] prefabs;

                // No models to assign.
                if (models == null || models.Length == 0)
                {
                    prefabs = new GameObject[0];
                }
                else
                {
                    var first = models.FirstOrDefault(x => x.CarType == item);

                    // No models for this car type.
                    if (first == default)
                    {
                        prefabs = new GameObject[0];
                    }
                    else
                    {
                        prefabs = first.Prefabs;
                    }
                }

                loadables.Add(new CargoType_v2.LoadableInfo(item.ToV2(), prefabs));
            }

            v2.loadableCarTypes = loadables.ToArray();
        }

        public static CargoType_v2 ToV2(this CustomCargo cargo)
        {
            var newCargo = ScriptableObject.CreateInstance<CargoType_v2>();

            newCargo.id = cargo.Identifier;
            newCargo.v1 = (CargoType)cargo.Value;

            newCargo.localizationKeyFull = cargo.LocalizationKeyFull;
            newCargo.localizationKeyShort = cargo.LocalizationKeyShort;

            newCargo.massPerUnit = cargo.MassPerUnit;
            newCargo.fullDamagePrice = cargo.FullDamagePrice;
            newCargo.environmentDamagePrice = cargo.EnvironmentDamagePrice;

            newCargo.requiredJobLicenses = cargo.Licenses.Select(x => ((JobLicenses)x).ToV2()).ToArray();
            newCargo.loadableCarTypes = Array.Empty<CargoType_v2.LoadableInfo>();

            return newCargo;
        }

        public static List<CargoType> ToCargoTypeGroup(this CustomCargoGroup group, string cargoName)
        {
            List<CargoType> types = new List<CargoType>();

            foreach (var item in group.CargosIds)
            {
                // Find cargo for the group using the id (name).
                if (!Globals.G.Types.cargos.TryFind(x => x.id == item, out var cargo))
                {
                    CCMod.Error($"Cargo '{item}' is missing (cargo group for '{cargoName}')");
                    continue;
                }

                types.Add(cargo.v1);
            }

            return types;
        }
    }
}
