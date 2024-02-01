using HarmonyLib;

namespace CC.Game
{
    [HarmonyPatch(typeof(SaveGameManager))]
    internal class SaveGameManagerPatches
    {
        [HarmonyPatch("DoSaveIO")]
        [HarmonyPrefix]
        public static void InjectSaveData(SaveGameData data)
        {
            SaveInjector.InjectDataIntoSaveGame(data);
        }

        [HarmonyPatch(nameof(SaveGameManager.FindStartGameData))]
        [HarmonyPostfix]
        public static void ExtractSaveData(SaveGameManager __instance)
        {
            SaveInjector.LoadedData = null;
            if (__instance.data == null) return;

            SaveInjector.ExtractDataFromSaveGame(__instance.data);
            CargoManager.ApplyMappings();
        }
    }
}
