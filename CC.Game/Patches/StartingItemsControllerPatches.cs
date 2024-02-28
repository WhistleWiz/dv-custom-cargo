using HarmonyLib;

namespace CC.Game.Patches
{
    [HarmonyPatch(typeof(StartingItemsController))]
    internal class StartingItemsControllerPatches
    {
        // Thanks Katycat and PaxJobs again.
        [HarmonyPostfix, HarmonyPatch(nameof(StartingItemsController.AddStartingItems))]
        private static void AddStartingItemsPostfix()
        {
            StateManager.RequestUnload();
        }
    }
}
