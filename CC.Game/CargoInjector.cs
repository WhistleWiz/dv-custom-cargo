using CC.Common;
using DV.Logic.Job;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using DV.Utils;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CC.Game
{
    internal static class CargoInjector
    {
        public static void InjectRoutes(CustomCargo cargo)
        {
            var srcStations = new List<StationController>();
            var destStations = new List<StationController>();

            foreach (var source in cargo.SourceStations)
            {
                if (!SingletonBehaviour<LogicController>.Instance.YardIdToStationController.TryGetValue(source, out var station))
                {
                    CCMod.Warning($"Could not find source station '{source}' for cargo '{cargo.Name}' " +
                        $"(is vanilla: {Helper.IsVanillaStation(source)})!");
                    continue;
                }

                srcStations.Add(station);
            }

            foreach (var destination in cargo.DestinationStations)
            {
                if (!SingletonBehaviour<LogicController>.Instance.YardIdToStationController.TryGetValue(destination, out var station))
                {
                    CCMod.Warning($"Could not find destination station '{destination}' for cargo '{cargo.Name}' " +
                        $"(is vanilla: {Helper.IsVanillaStation(destination)})!");
                    continue;
                }

                destStations.Add(station);
            }

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

            foreach (var station in srcStations)
            {
                foreach (var group in cargo.CargoGroups)
                {
                    station.proceduralJobsRuleset.outputCargoGroups.Add(new CargoGroup(
                        CargoManager.ToCargoTypeGroup(group, cargo.Name),
                        destStations));
                }

                AddCargoToWarehouse(station, cargo);
            }

            foreach (var station in destStations)
            {
                foreach (var group in cargo.CargoGroups)
                {
                    station.proceduralJobsRuleset.inputCargoGroups.Add(new CargoGroup(
                        CargoManager.ToCargoTypeGroup(group, cargo.Name),
                        srcStations));
                }

                AddCargoToWarehouse(station, cargo);
            }
        }

        private static void AddCargoToWarehouse(StationController station, CustomCargo cargo)
        {
            CCMod.Log($"Adding cargo to station warehouse: '{station.name}'");
            var machine = station.logicStation.yard.WarehouseMachines.First();

            if (machine != null)
            {
                var controller = WarehouseMachineController.allControllers.First(c => c.warehouseMachine == machine);
                machine.SupportedCargoTypes.Add((CargoType)cargo.Id);
                controller.supportedCargoTypes.Add((CargoType)cargo.Id);
            }
        }
    }
}
