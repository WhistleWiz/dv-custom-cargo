using DV.Utils;
using HarmonyLib;

namespace CC.Game
{
    [HarmonyPatch(typeof(StationProceduralJobsController))]
    internal static class StationProceduralJobsControllerPatches
    {
        private static bool s_hasRun = false;

        static StationProceduralJobsControllerPatches()
        {
            // So every time a save is loaded we can make sure
            // the patch is run again.
            UnloadWatcher.UnloadRequested += () => s_hasRun = false;
        }

        [HarmonyPatch(nameof(StationProceduralJobsController.TryToGenerateJobs))]
        [HarmonyPrefix]
        public static void TryToGenerateJobsPrefix()
        {
            // Only run once per load.
            if (s_hasRun)
            {
                return;
            }

            s_hasRun = true;
            CCMod.Log("First attempt at job generation started, injecting new routes before it runs...");
            CCMod.Log(string.Join(", ", SingletonBehaviour<LogicController>.Instance.YardIdToStationController.Keys));

            foreach (var item in CargoManager.AddedCargos)
            {
                CCMod.Log($"Injecting routes for cargo '{item.Name}'...");
                CargoInjector.InjectRoutes(item);
            }
        }
    }
}
