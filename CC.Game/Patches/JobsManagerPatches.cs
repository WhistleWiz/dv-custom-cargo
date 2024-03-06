using DV.Logic.Job;
using HarmonyLib;
using System;

namespace CC.Game.Patches
{
    [HarmonyPatch(typeof(JobsManager))]
    internal static class JobsManagerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(JobsManager.AllowAutoCreate))]
        public static void AllowAutoCreatePostfix()
        {
            StateManager.RequestLoad();
            CargoInjector.StartInjection();
        }
    }

    [HarmonyPatch(typeof(JobSaveManager))]
    internal static class JobSaveManagerPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(JobSaveManager.LoadJobSaveGameData))]
        public static void LoadJobSaveGameDataPrefix()
        {
            StateManager.RequestLoad();
            CargoInjector.StartInjection();
        }
    }
}
