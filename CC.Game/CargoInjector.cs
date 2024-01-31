using CC.Common;
using DV.ThingTypes;
using DV.Utils;
using System.Collections.Generic;
using System.Linq;

namespace CC.Game
{
    internal static class CargoInjector
    {
        public static void InjectRoutes(CustomCargo cargo)
        {
            // Find the stations where we need to inject new routes.
            var srcStations = new List<StationController>();
            var destStations = new List<StationController>();

            foreach (var source in cargo.SourceStations)
            {
                if (!SingletonBehaviour<LogicController>.Instance.YardIdToStationController.TryGetValue(source, out var station))
                {
                    CCMod.Warning($"Could not find source station '{source}' for cargo '{cargo.Identifier}' " +
                        $"(is vanilla: {Helper.IsVanillaStation(source)})!");
                    continue;
                }

                srcStations.Add(station);
            }

            foreach (var destination in cargo.DestinationStations)
            {
                if (!SingletonBehaviour<LogicController>.Instance.YardIdToStationController.TryGetValue(destination, out var station))
                {
                    CCMod.Warning($"Could not find destination station '{destination}' for cargo '{cargo.Identifier}' " +
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
                foreach (var group in cargo.CargoGroups)
                {
                    station.proceduralJobsRuleset.outputCargoGroups.Add(new CargoGroup(
                        CargoManager.ToCargoTypeGroup(group, cargo.Identifier),
                        destStations));
                }

                AddCargoToWarehouse(station, cargo);
            }

            foreach (var station in destStations)
            {
                foreach (var group in cargo.CargoGroups)
                {
                    station.proceduralJobsRuleset.inputCargoGroups.Add(new CargoGroup(
                        CargoManager.ToCargoTypeGroup(group, cargo.Identifier),
                        srcStations));
                }

                AddCargoToWarehouse(station, cargo);
            }
        }

        private static void AddCargoToWarehouse(StationController station, CustomCargo cargo)
        {
            // Grab only one warehouse machine at the station.
            CCMod.Log($"Adding cargo to station warehouse: '{station.name}'");
            var machine = station.logicStation.yard.WarehouseMachines.First();

            if (machine != null)
            {
                // Get the controller for the machine we are using.
                var controller = WarehouseMachineController.allControllers.First(c => c.warehouseMachine == machine);
                // Add the cargo to the list of supported cargos.
                machine.SupportedCargoTypes.Add((CargoType)cargo.Value);
                controller.supportedCargoTypes.Add((CargoType)cargo.Value);
            }
        }
    }
}
