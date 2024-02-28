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
        public static Dictionary<string, int> Mapping = new Dictionary<string, int>();
        public static List<(CustomCargo Custom, CargoType_v2 V2)> AddedCargos = new List<(CustomCargo, CargoType_v2)>();

        private static int s_tempValue = Constants.DefaultCargoValue;
        private static AssetBundle? s_commonBundle;

        public static void LoadCargos(UnityModManager.ModEntry mod)
        {
            List<CargoType_v2> newCargos = new List<CargoType_v2>();

            // Find the 'cargo.json' files.
            foreach (string jsonPath in Directory.EnumerateFiles(mod.Path, Constants.CargoFile, SearchOption.AllDirectories))
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

            // Unload the common bundle at the end.
            if (s_commonBundle != null)
            {
                s_commonBundle.Unload(false);
                s_commonBundle = null;
            }

            // If we did load any cargo...
            if (newCargos.Count > 0)
            {
                CCMod.Log($"Loaded {newCargos.Count} cargos from {mod.Path}");

                foreach (var item in newCargos)
                {
                    CCMod.Log($"{item.id}");
                }
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

            // Ensure the cargo that defines the groups is in them.
            foreach (var group in c.CargoGroups)
            {
                group.AddIdIfMissing(c.Identifier);
            }

            // Convert into actual cargo and add it.
            v2 = c.ToV2();
            Globals.G.Types.cargos.Add(v2);
            AddedCargos.Add((c, v2));

            // Add translations for this cargo.
            AddTranslations(c);

            Sprite? icon = null;
            Sprite? resourceIcon = null;

            // Try to load any asset bundle.
            TryLoadBundle(c, directory, out var models, ref icon, ref resourceIcon);

            // Try to load icon files.
            TryLoadIcons(directory, ref icon, ref resourceIcon);

            if (icon != null)
            {
                v2.icon = icon;
            }

            if (resourceIcon != null)
            {
                v2.resourceIcon = resourceIcon;
            }

            // Add vanilla types to loadable info.
            AddLoadableInfo(c, v2, models);

            return true;
        }

        private static bool TryLoadBundle(CustomCargo c, string directory, out ModelsForVanillaCar[] models, ref Sprite? icon, ref Sprite? resourceIcon)
        {
            var assetBundlePath = Path.Combine(directory, Constants.ModelBundle);
            var commonPath = Path.Combine(Path.GetDirectoryName(directory), Constants.ModelBundle);
            bool usingCommon = false;
            AssetBundle assetBundle;

            if (!File.Exists(assetBundlePath))
            {
                if (s_commonBundle != null)
                {
                    assetBundle = s_commonBundle;
                    usingCommon = true;
                }
                else if (File.Exists(commonPath))
                {
                    CCMod.Log($"Loading common model bundle...");
                    s_commonBundle = AssetBundle.LoadFromFile(commonPath);
                    assetBundle = s_commonBundle;
                    usingCommon = true;
                }
                else
                {
                    CCMod.Log($"No model bundle found (expected {assetBundlePath} or {commonPath}).");
                    models = null!;
                    icon = null!;
                    resourceIcon = null!;
                    return false;
                }
            }
            else
            {
                CCMod.Log($"Loading model bundle...");
                assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            }

            if (assetBundle == null)
            {
                CCMod.Error($"Failed to load model bundle!");
                models = null!;
                icon = null!;
                resourceIcon = null!;
                return false;
            }

            if (usingCommon)
            {
                var target = assetBundle.LoadAllAssets<CommonCargoObject>().FirstOrDefault(x => x.Identifier == c.Identifier);

                if (target == null)
                {
                    models = null!;
                    icon = null!;
                    resourceIcon = null!;
                    CCMod.Error($"There was no cargo {c.Identifier} in the common bundle!");
                    return false;
                }

                models = target.Models.ToArray();
                icon = target.Icon;
                resourceIcon = target.ResourceIcon;
            }
            else
            {
                models = assetBundle.LoadAllAssets<ModelsForVanillaCar>();
                icon = assetBundle.LoadAsset<Sprite>(Constants.Icon);
                resourceIcon = assetBundle.LoadAsset<Sprite>(Constants.ResourceIcon);

                assetBundle.Unload(false);
            }

            foreach (var item in models)
            {
                foreach (var prefab in item.Prefabs)
                {
                    // Ask CCL to handle some model loading here, so we can support
                    // the proxy system on custom cargo.
                }
            }

            return true;
        }

        private static void TryLoadIcons(string directory, ref Sprite? icon, ref Sprite? resourceIcon)
        {
            byte[] data;

            // Path to the image.
            var path = Path.Combine(directory, Constants.IconFile);

            if (icon == null && File.Exists(path))
            {
                // Shove the raw bytes of the image into the texture.
                // Texture size is not important and will be automatically changed.
                data = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                // Create a sprite that covers the whole texture.
                icon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
            }

            // Repeat for the resource icon.
            path = Path.Combine(directory, Constants.ResourceIconFile);

            if (resourceIcon == null && File.Exists(path))
            {
                data = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                resourceIcon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
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

                ProcessOriginalPrefabs(prefabs);
                loadables.Add(new CargoType_v2.LoadableInfo(item.ToV2(), prefabs));
            }

            v2.loadableCarTypes = loadables.ToArray();
        }

        private static void ProcessOriginalPrefabs(GameObject[] prefabs)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                // If one of the prefabs is actually to be replaced...
                if (prefabs[i].TryGetComponent<UseCargoPrefab>(out var comp))
                {
                    prefabs[i] = CargoPrefab.All[comp.PrefabIndex].ToPrefab();
                }
            }
        }

        public static CargoType_v2 ToV2(this CustomCargo cargo)
        {
            var newCargo = ScriptableObject.CreateInstance<CargoType_v2>();

            newCargo.id = cargo.Identifier;
            newCargo.v1 = (CargoType)(++s_tempValue);

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

                // Don't include it either if there's no wagon to load it.
                if (cargo.loadableCarTypes.Length == 0)
                {
                    CCMod.Error($"Cargo '{item}' cannot be loaded on any car (cargo group for '{cargoName}')");
                    continue;
                }

                types.Add(cargo.v1);
            }

            return types;
        }

        public static void ApplyMappings()
        {
            CCMod.Log("Applying mappings...");

            int highest = Constants.DefaultCargoValue;

            // Get the highest ID, to start counting from there.
            foreach (var item in Mapping)
            {
                highest = Mathf.Max(highest, (int)item.Value);
            }

            AddedValues = new HashSet<CargoType>();
            int newTypes = 0;

            foreach (var (_, v2) in AddedCargos)
            {
                if (!Mapping.TryGetValue(v2.id, out int type))
                {
                    type = ++highest;
                    Mapping.Add(v2.id, type);
                    newTypes++;
                }

                v2.v1 = (CargoType)type;
                AddedValues.Add((CargoType)type);
            }

            // Recalculate caches with the new values.
            Globals.G.Types.RecalculateCaches();
            CCMod.Log($"Mappings applied: {AddedValues.Count}/{Mapping.Count} (new: {newTypes}), highest value is {highest}");
        }
    }
}
