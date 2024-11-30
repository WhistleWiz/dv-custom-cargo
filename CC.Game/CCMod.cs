using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DVLangHelper.Runtime;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace CC.Game
{
	public static class CCMod
    {
        public const string Guid = "wiz.customcargo";

        public static UnityModManager.ModEntry Instance { get; private set; } = null!;
        public static TranslationInjector Translations { get; private set; } = null!;
        public static Dictionary<string, Material> MaterialCache { get; private set; } = null!;
        public static Dictionary<string, Mesh> MeshCache { get; private set; } = null!;

        // Unity Mod Manager Wiki: https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = modEntry;
            Translations = new TranslationInjector(Guid);

            ScanMods();
            UnityModManager.toggleModsListen += HandleModToggled;

            var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

            BuildCache();

			return true;
		}

		private static void ScanMods()
		{
            // Scan mods loaded before for cargo.
            foreach (var mod in UnityModManager.modEntries)
            {
                if (mod.Active)
                {
                    CargoManager.LoadCargos(mod);
                }
            }
        }

        private static void HandleModToggled(UnityModManager.ModEntry modEntry, bool newState)
        {
            // For each new loaded mod, check for cargo.
            if (newState)
            {
                CargoManager.LoadCargos(modEntry);
            }
        }

        public static void Log(string message)
		{
			Instance.Logger.Log(message);
		}

        public static void Warning(string message)
        {
            Instance.Logger.Warning(message);
        }

        public static void Error(string message)
        {
            Instance.Logger.Error(message);
        }

        private static void BuildCache()
        {
            // For timing.
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            MaterialCache = new Dictionary<string, Material>();

            // For materials, first group by material name.
            var mats = Resources.FindObjectsOfTypeAll<Material>()
                .Where(x => x != null)
                .GroupBy(x => x.name, StringComparer.Ordinal);

            foreach (var group in mats)
            {
                // If there's only 1 material per name, add it directly.
                if (group.Count() == 1)
                {
                    MaterialCache.Add(group.Key, group.First());
                    continue;
                }

                // For multiple materials with the same name (ex. Glass), group by the texture name too.
                var organised = group.GroupBy(x => GetSortingNameMaterial(x), StringComparer.Ordinal);

                // Finally add the first element of these new groups.
                foreach (var newGroup in organised)
                {
                    // Same for materials with no texture, just take the first one.
                    if (string.IsNullOrEmpty(newGroup.Key))
                    {
                        MaterialCache.Add(group.Key, newGroup.First());
                        continue;
                    }

                    MaterialCache.Add($"{group.Key}/{newGroup.Key}", newGroup.First());
                }
            }

            sw.Stop();
            Log($"Built material cache [{sw.Elapsed.TotalSeconds:F4}s]");
            //Log($"\"{string.Join("\",\n\"", MaterialCache.Keys.OrderBy(x => x))}\"");
            sw.Restart();

            MeshCache = new Dictionary<string, Mesh>();

            var meshes = Resources.FindObjectsOfTypeAll<Mesh>()
                .Where(x => x != null)
                .GroupBy(x => x.name, StringComparer.Ordinal);

            foreach (var group in meshes)
            {
                if (group.Count() == 1)
                {
                    AddToMeshCache(group.Key, group.First());
                    continue;
                }

                // For multiple meshes with the same name (ex. cab), group by the vertex count too.
                var organised = group.GroupBy(x => GetSortingNameMesh(x), StringComparer.Ordinal);

                foreach (var newGroup in organised)
                {
                    if (string.IsNullOrEmpty(newGroup.Key))
                    {
                        AddToMeshCache(group.Key, newGroup.First());
                        continue;
                    }

                    AddToMeshCache($"{group.Key}/{newGroup.Key}", newGroup.First());
                }
            }

            sw.Stop();
            Log($"Built mesh cache [{sw.Elapsed.TotalSeconds:F4}s]");
            //Log($"\"{string.Join("\",\n\"", MeshCache.Keys.OrderBy(x => x))}\"");

            static string GetSortingNameMaterial(Material mat)
            {
                var tex = mat.mainTexture;

                if (tex != null) return tex.name;

                return string.Empty;
            }

            static string GetSortingNameMesh(Mesh mesh)
            {
                return mesh.vertexCount.ToString();
            }

            static void AddToMeshCache(string name, Mesh mesh)
            {
                // There are a LOT of meshes, further group them by initial.
                if (string.IsNullOrEmpty(name) || !char.IsLetter(name, 0))
                {
                    MeshCache.Add("OTHER/" + name, mesh);
                    return;
                }

                MeshCache.Add($"{char.ToUpper(name[0])}/{name}", mesh);
            }
        }
    }
}
