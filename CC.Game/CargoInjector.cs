using CC.Common;
using DV.ThingTypes;
using DV.Utils;
using System.Collections.Generic;
using System.Linq;

namespace CC.Game
{
    internal static class CargoInjector
    {
        public static void InjectRoutes(CustomCargo cc, CargoType_v2 ct)
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

        public static void InjectEffects(CustomCargo cc, CargoType_v2 ct)
        {
            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.Oils))
            {
                TrainCarAndCargoDamageProperties.Oils.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.Liquids))
            {
                TrainCarAndCargoDamageProperties.Liquids.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.FlammableLiquids))
            {
                TrainCarAndCargoDamageProperties.FlammableLiquids.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.CorrosiveLiquids))
            {
                TrainCarAndCargoDamageProperties.CorosiveLiquids.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.Gases))
            {
                TrainCarAndCargoDamageProperties.Gases.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.FlammableGases))
            {
                TrainCarAndCargoDamageProperties.FlammableGases.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.ExtiguishingGases))
            {
                TrainCarAndCargoDamageProperties.ExtinguishingGases.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.FlammableSolids))
            {
                TrainCarAndCargoDamageProperties.FlammableSolids.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.RadioactiveCargo))
            {
                TrainCarAndCargoDamageProperties.RadioactiveCargo.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.ExplosiveCargo))
            {
                TrainCarAndCargoDamageProperties.ExplosiveCargo.Add(ct.v1);
            }

            if (cc.CargoEffectPools.HasFlag(CargoEffectPools.Oxidizers))
            {
                TrainCarAndCargoDamageProperties.Oxidizers.Add(ct.v1);
            }
        }

        public static void InjectLeakProperties(CustomCargo cc, CargoType_v2 ct)
        {

        }
    }
}
