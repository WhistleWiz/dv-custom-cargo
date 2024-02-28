using HarmonyLib;

namespace CC.Game.Patches
{
    [HarmonyPatch(typeof(StationProceduralJobsController))]
    internal class StationProceduralJobsControllerPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(StationProceduralJobsController.TryToGenerateJobs))]
        public static void TryToGenerateJobsPrefix()
        {
            StateManager.RequestLoad();
            CargoInjector.StartInjection();
        }
    }
}
