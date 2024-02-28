using HarmonyLib;

namespace CC.Game.Patches
{
    [HarmonyPatch(typeof(SaveGameManager))]
    internal class SaveGameManagerPatches
    {
        [HarmonyPrefix, HarmonyPatch("DoSaveIO")]
        public static void InjectSaveData(SaveGameData data)
        {
            SaveInjector.InjectDataIntoSaveGame(data);
        }

        [HarmonyPostfix, HarmonyPatch(nameof(SaveGameManager.FindStartGameData))]
        public static void ExtractSaveData(SaveGameManager __instance)
        {
            SaveInjector.LoadedData = null;
            if (__instance.data == null) return;

            SaveInjector.ExtractDataFromSaveGame(__instance.data);
            CargoManager.ApplyMappings();
        }
    }
}
