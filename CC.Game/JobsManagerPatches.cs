﻿using DV.Logic.Job;
using DV.ThingTypes;
using DV.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            CCMod.Log("Stations in map: " + string.Join(", ", SingletonBehaviour<LogicController>.Instance.YardIdToStationController.Keys));

            // Inject routes for each cargo the mod loaded.
            foreach (var (custom, v2) in CargoManager.AddedCargos)
            {
                CCMod.Log($"Injecting routes for cargo '{v2.id}'...");
                CargoInjector.InjectRoutes(custom, v2);
            }

            // Remake station to car type cache.
            CCMod.Log("Remaking station to car type mapping...");
            Dictionary<StationController, HashSet<TrainCarType_v2>> s2ct = new Dictionary<StationController, HashSet<TrainCarType_v2>>();

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var type = SingletonBehaviour<LogicController>.Instance.GetType();
            var original = type.GetField("stationToSupportedCarTypes", flags);
            var method = type.GetMethod("GetCarTypesThatStationUses", flags);

            foreach (var item in SingletonBehaviour<LogicController>.Instance.YardIdToStationController.Values)
            {
                s2ct.Add(item, (HashSet<TrainCarType_v2>)method.Invoke(SingletonBehaviour<LogicController>.Instance, new[] { item }));
            }

            original.SetValue(SingletonBehaviour<LogicController>.Instance, s2ct);
        }
    }
}
