using DV.Logic.Job;
using DV.Utils;
using HarmonyLib;

namespace CC.Game
{
    [HarmonyPatch(typeof(JobsManager))]
    internal static class JobsManagerPatches
    {
        private static bool s_hasRun = false;

        static JobsManagerPatches()
        {
            // So every time a save is loaded we can make sure
            // the patch is run again.
            UnloadWatcher.UnloadRequested += () => s_hasRun = false;
        }

        // JobsManager doesn't actually have Awake(), however this one
        // is called within Awake() of the base class (SingletonBehaviour),
        // which ends up being effectively the same thing.
        [HarmonyPatch(nameof(JobsManager.AllowAutoCreate))]
        [HarmonyPostfix]
        public static void AllowAutoCreatePostfix()
        {
            // Only run once per load.
            if (s_hasRun)
            {
                return;
            }

            s_hasRun = true;
            CCMod.Log("First attempt at job generation started, injecting new routes before it runs...");
            CCMod.Log(string.Join(", ", SingletonBehaviour<LogicController>.Instance.YardIdToStationController.Keys));

            // Inject routes for each cargo the mod loaded.
            foreach (var item in CargoManager.AddedCargos)
            {
                CCMod.Log($"Injecting routes for cargo '{item.Identifier}'...");
                CargoInjector.InjectRoutes(item);
            }
        }
    }
}
