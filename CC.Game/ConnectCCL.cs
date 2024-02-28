using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CC.Game
{
    internal static class ConnectCCL
    {
        private static MethodInfo? s_method;
        private static bool s_loaded = false;

        public static bool LoadedCCL => s_loaded;

        static ConnectCCL()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\Mods\\DVCustomCarLoader\\CCL.Importer.dll";

            if (!File.Exists(path))
            {
                CCMod.Warning($"Could not load CCL (CCL is not installed!)");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFile(path);
                Type t = assembly.GetType("CCL.Importer.Processing.ModelProcessor");
                s_method = t.GetMethod("DoBasicProcessing");
            }
            catch (Exception e)
            {
                CCMod.Warning($"Could not load CCL ({e.Message})");
                return;
            }

            CCMod.Log("Successfuly loaded CCL integration!");
            s_loaded = true;
        }

        public static void ProcessPrefab(GameObject prefab)
        {
            if (!LoadedCCL)
            {
                return;
            }

            if (s_method == null)
            {
                CCMod.Error("CCL call failed! Setting CCL load to false.");
                s_loaded = false;
                return;
            }

            s_method?.Invoke(null, new object[] { prefab });
        }
    }
}
