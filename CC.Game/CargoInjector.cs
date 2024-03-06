using CC.Common;
using CC.Common.Effects;
using DV.Localization;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using DV.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CC.Game
{
    internal static class CargoInjector
    {
        public static void StartInjection()
        {
            // Only run once per load.
            if (StateManager.IsInjected)
            {
                CCMod.Log("Already injected, returning...");
                return;
            }

            StateManager.IsInjected = true;

            CCMod.Log("First attempt at job generation started, injecting new routes before it runs...");
            CCMod.Log("Stations in map: " + string.Join(", ", LogicController.Instance.YardIdToStationController.Keys));

            foreach (var (custom, v2) in CargoManager.AddedCargos)
            {
                // Inject routes for each cargo the mod loaded.
                CCMod.Log($"Injecting routes for cargo '{v2.id}'...");
                InjectRoutes(custom, v2);

                // Inject the properties into the caches.
                CCMod.Log($"Injecting properties for cargo '{v2.id}'...");
                InjectProperties(custom, v2);

                if (custom.Properties.CargoEffectPools != CargoEffectPools.None)
                {
                    // Inject cargo effects into the caches if the cargo is in a pool.
                    CCMod.Log($"Injecting effects for cargo '{v2.id}'...");
                    InjectEffects(custom, v2);
                }
            }

            // Reset caches.
            RemakeStationToCarTypeCache();
            ClearFlammables();

            CCMod.Log($"Injection finished!");
        }

        private static void RemakeStationToCarTypeCache()
        {
            CCMod.Log("Remaking station to car type mapping...");
            Dictionary<StationController, HashSet<TrainCarType_v2>> s2ct = new Dictionary<StationController, HashSet<TrainCarType_v2>>();

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var type = LogicController.Instance.GetType();
            var original = type.GetField("stationToSupportedCarTypes", flags);
            var method = type.GetMethod("GetCarTypesThatStationUses", flags);

            foreach (var item in LogicController.Instance.YardIdToStationController.Values)
            {
                s2ct.Add(item, (HashSet<TrainCarType_v2>)method.Invoke(LogicController.Instance, new[] { item }));
            }

            original.SetValue(LogicController.Instance, s2ct);
        }

        private static void InjectRoutes(CustomCargo cc, CargoType_v2 ct)
        {
            if (ct.loadableCarTypes.Length == 0)
            {
                CCMod.Error("Cargo has no loadable car types! Skipping injection...");
                return;
            }

            foreach (var group in cc.CargoGroups)
            {
                CCMod.Log($"Group: {string.Join(", ", group.CargosIds)}");

                // Find the stations where we need to inject new routes.
                var srcStations = new List<StationController>();
                var srcTracks = new List<string>();
                var destStations = new List<StationController>();
                var destTracks = new List<string>();

                foreach (var source in group.SourceStations)
                {
                    var yard = GetStationName(source);

                    if (!LogicController.Instance.YardIdToStationController.TryGetValue(yard, out var station))
                    {
                        CCMod.Warning($"Could not find source station '{yard}' for cargo '{cc.Identifier}' " +
                            $"(is vanilla: {Helper.IsVanillaStation(yard)})!");
                        continue;
                    }

                    srcStations.Add(station);
                    srcTracks.Add(source);
                }

                // We only need each station once, even if it supports multiple tracks.
                srcStations = srcStations.Distinct().ToList();

                foreach (var destination in group.DestinationStations)
                {
                    var yard = GetStationName(destination);

                    if (!LogicController.Instance.YardIdToStationController.TryGetValue(yard, out var station))
                    {
                        CCMod.Warning($"Could not find destination station '{yard}' for cargo '{cc.Identifier}' " +
                            $"(is vanilla: {Helper.IsVanillaStation(yard)})!");
                        continue;
                    }

                    destStations.Add(station);
                    destTracks.Add(destination);
                }

                destStations = destStations.Distinct().ToList();

                // If there's no source or destination stations, don't add any routes.
                if (srcStations.Count == 0)
                {
                    CCMod.Error("Cargo has no source stations! Skipping injection...");
                    return;
                }

                if (destStations.Count == 0)
                {
                    CCMod.Error("Cargo has no destination stations! Skipping injection...");
                    return;
                }

                // Add the cargo group to each source station, then also add
                // the cargos in it to the station's warehouse machine.
                var ctgroup = Extensions.ToCargoTypeGroup(group, cc.Identifier);

                foreach (var station in srcStations)
                {
                    station.proceduralJobsRuleset.outputCargoGroups.Add(new CargoGroup(ctgroup, destStations));

                    foreach (var v1 in ctgroup)
                    {
                        AddCargoToWarehouses(station, srcTracks, v1);
                    }
                }

                foreach (var station in destStations)
                {
                    station.proceduralJobsRuleset.inputCargoGroups.Add(new CargoGroup(ctgroup, srcStations));

                    foreach (var v1 in ctgroup)
                    {
                        AddCargoToWarehouses(station, destTracks, v1);
                    }
                }
            }
        }

        private static string GetStationName(string text)
        {
            // This should turn stations in any format into their names only.
            // HB       -> HB
            // OWN      -> OWN
            // FM-A1L   -> FM
            var splits = text.Split('-');
            return string.Join("-", splits.Take(Mathf.Max(1, splits.Length - 1)));
        }

        private static void AddCargoToWarehouses(StationController station, List<string> tracksToLoad, CargoType cargo)
        {
            // Add cargo to all machines...
            foreach (var machine in station.logicStation.yard.WarehouseMachines)
            {
                if (machine != null && !machine.SupportedCargoTypes.Contains(cargo))
                {
                    var id = machine.WarehouseTrack.ID.FullDisplayID;

                    // ...if that machine is included in the accepted tracks.
                    if (!tracksToLoad.Any(x => id.StartsWith(x)))
                    {
                        continue;
                    }

                    var v2 = cargo.ToV2();

                    CCMod.Log($"Adding cargo '{v2.id}' to station warehouse at {id}");

                    // Get the controller for the machine we are using.
                    var controller = WarehouseMachineController.allControllers.First(c => c.warehouseMachine == machine);
                    // Add the cargo to the list of supported cargos.
                    machine.SupportedCargoTypes.Add(cargo);
                    controller.supportedCargoTypes.Add(cargo);
                    InjectCargoName(controller, v2);
                    controller.UpdateScreen();
                }
            }
        }

        private static void InjectCargoName(WarehouseMachineController controller, CargoType_v2 cargo)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var type = typeof(WarehouseMachineController);
            var original = type.GetField("supportedCargoTypesText", flags);

            string text = LocalizationAPI.L(cargo.localizationKeyFull, new string[0]);

            original.SetValue(controller, $"{(string)original.GetValue(controller)}{text}\n");
        }

        private static void InjectProperties(CustomCargo cc, CargoType_v2 ct)
        {
            TrainCarAndCargoDamageProperties.CargoDamageProperties.Add(ct.v1, cc.Properties.ToDV());
            TrainCarAndCargoDamageProperties.CargoLeakProperties.Add(ct.v1, cc.Properties.LeakProperties.ToDV());
            TrainCarAndCargoDamageProperties.CargoReactionProperties.Add(ct.v1, cc.Properties.ReactionProperties.ToDV());
        }

        private static void InjectEffects(CustomCargo cc, CargoType_v2 ct)
        {
            // Add the cargo to the corresponding pools.
            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.Oils))
            {
                TrainCarAndCargoDamageProperties.Oils.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.Liquids))
            {
                TrainCarAndCargoDamageProperties.Liquids.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.FlammableLiquids))
            {
                TrainCarAndCargoDamageProperties.FlammableLiquids.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.CorrosiveLiquids))
            {
                TrainCarAndCargoDamageProperties.CorosiveLiquids.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.Gases))
            {
                TrainCarAndCargoDamageProperties.Gases.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.FlammableGases))
            {
                TrainCarAndCargoDamageProperties.FlammableGases.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.ExtiguishingGases))
            {
                TrainCarAndCargoDamageProperties.ExtinguishingGases.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.FlammableSolids))
            {
                TrainCarAndCargoDamageProperties.FlammableSolids.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.RadioactiveCargo))
            {
                TrainCarAndCargoDamageProperties.RadioactiveCargo.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.ExplosiveCargo))
            {
                TrainCarAndCargoDamageProperties.ExplosiveCargo.Add(ct.v1);
            }

            if (cc.Properties.CargoEffectPools.HasFlag(CargoEffectPools.Oxidizers))
            {
                TrainCarAndCargoDamageProperties.Oxidizers.Add(ct.v1);
            }
        }

        private static void ClearFlammables()
        {
            CCMod.Log("Clearing cached flammables...");
            Type t = typeof(TrainCarAndCargoDamageProperties);
            FieldInfo fi = t.GetField("_flammableCargo", BindingFlags.Static | BindingFlags.NonPublic);
            var cargo = (HashSet<CargoType>)fi.GetValue(null);
            cargo?.Clear();
        }
    }
}
