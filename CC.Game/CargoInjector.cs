using CC.Common;
using DV.Logic.Job;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System;
using System.Linq;
using UnityEngine;

namespace CC.Game
{
    internal static class CargoInjector
    {
        public static CargoType_v2 ToV2(this CustomCargo cargo)
        {
            var newCargo = ScriptableObject.CreateInstance<CargoType_v2>();

            newCargo.id = cargo.Name;
            newCargo.v1 = (CargoType)cargo.Id;

            newCargo.localizationKeyFull = cargo.LocalizationKeyFull;
            newCargo.localizationKeyShort = cargo.LocalizationKeyShort;

            newCargo.massPerUnit = cargo.MassPerUnit;
            newCargo.fullDamagePrice = cargo.FullDamagePrice;
            newCargo.environmentDamagePrice = cargo.EnvironmentDamagePrice;

            newCargo.requiredJobLicenses = cargo.Licenses.Select(x => ((JobLicenses)x).ToV2()).ToArray();
            newCargo.loadableCarTypes = Array.Empty<CargoType_v2.LoadableInfo>();

            return newCargo;
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
