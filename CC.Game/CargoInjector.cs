using CC.Common;
using CC.Common.Effects;
using DV.ThingTypes;
using DV.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            CCMod.Log("First attempt at job generation started, injecting new routes before it runs...");
            CCMod.Log("Stations in map: " + string.Join(", ", SingletonBehaviour<LogicController>.Instance.YardIdToStationController.Keys));

            foreach (var (custom, v2) in CargoManager.AddedCargos)
            {
                // Inject routes for each cargo the mod loaded.
                CCMod.Log($"Injecting routes for cargo '{v2.id}'...");
                InjectRoutes(custom, v2);

                if (custom.Properties != null)
                {
                    // Inject cargo effects into the caches.
                    CCMod.Log($"Injecting effects for cargo '{v2.id}'...");
                    InjectEffects(custom, v2);
                }
            }

            RemakeStationToCarTypeCache();

            StateManager.IsInjected = true;
        }

        private static void RemakeStationToCarTypeCache()
        {
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

        private static void InjectRoutes(CustomCargo cc, CargoType_v2 ct)
        {
            if (ct.loadableCarTypes.Length == 0)
            {
                CCMod.Error("Cargo has no loadable car types! Skipping injection...");
                return;
            }

            // Find the stations where we need to inject new routes.
            var srcStations = new List<StationController>();
            var destStations = new List<StationController>();

            foreach (var source in cc.SourceStations)
            {
                if (!SingletonBehaviour<LogicController>.Instance.YardIdToStationController.TryGetValue(source, out var station))
                {
                    CCMod.Warning($"Could not find source station '{source}' for cargo '{cc.Identifier}' " +
                        $"(is vanilla: {Helper.IsVanillaStation(source)})!");
                    continue;
                }

                srcStations.Add(station);
            }

            foreach (var destination in cc.DestinationStations)
            {
                if (!SingletonBehaviour<LogicController>.Instance.YardIdToStationController.TryGetValue(destination, out var station))
                {
                    CCMod.Warning($"Could not find destination station '{destination}' for cargo '{cc.Identifier}' " +
                        $"(is vanilla: {Helper.IsVanillaStation(destination)})!");
                    continue;
                }

                destStations.Add(station);
            }

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

            // Add each cargo group to each source station, then also add
            // the cargo to the station's warehouse machine.
            foreach (var station in srcStations)
            {
                foreach (var group in cc.CargoGroups)
                {
                    station.proceduralJobsRuleset.outputCargoGroups.Add(new CargoGroup(
                        CargoManager.ToCargoTypeGroup(group, cc.Identifier),
                        destStations));
                }

                AddCargoToWarehouse(station, ct.v1);
            }

            foreach (var station in destStations)
            {
                foreach (var group in cc.CargoGroups)
                {
                    station.proceduralJobsRuleset.inputCargoGroups.Add(new CargoGroup(
                        CargoManager.ToCargoTypeGroup(group, cc.Identifier),
                        srcStations));
                }

                AddCargoToWarehouse(station, ct.v1);
            }
        }

        private static void AddCargoToWarehouse(StationController station, CargoType cargo)
        {
            // Grab only one warehouse machine at the station.
            CCMod.Log($"Adding cargo to station warehouse: '{station.name}'");
            var machine = station.logicStation.yard.WarehouseMachines.First();

            if (machine != null)
            {
                // Get the controller for the machine we are using.
                var controller = WarehouseMachineController.allControllers.First(c => c.warehouseMachine == machine);
                // Add the cargo to the list of supported cargos.
                machine.SupportedCargoTypes.Add(cargo);
                controller.supportedCargoTypes.Add(cargo);
            }
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
    }
}
