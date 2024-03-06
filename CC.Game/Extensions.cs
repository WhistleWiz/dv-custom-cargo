using CC.Common;
using CC.Game;
using DV;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CC.Game
{
    internal static class Extensions
    {
        private static int s_tempValue = Constants.DefaultCargoValue;

        public static TrainCarType_v2 ToV2(this CarParentType car)
        {
            return ((TrainCarType)car).ToV2().parentType;
        }

        public static GameObject ToPrefab(this CargoPrefab cargo)
        {
            // Assume it can be translated directly.
            if (cargo.TypeEnum < Constants.DefaultCargoValue)
            {
                return ((CargoType)cargo.TypeEnum).ToV2().loadableCarTypes[cargo.CarIndex].cargoPrefabVariants[cargo.VariantIndex];
            }

            return null!;
        }

        public static CargoDamageProperties ToDV(this CargoHazmatProperties properties)
        {
            return new CargoDamageProperties(
                properties.MaxHealth,
                properties.DamageTolerance,
                properties.DamageMultiplier,
                properties.DamageResistance,
                properties.FireDamageMultiplier,
                properties.FireResistance);
        }

        public static CargoLeakProperties ToDV(this LeakProperties properties)
        {
            return new CargoLeakProperties(
                properties.MaxLeakFlow,
                properties.MinLeakFlow,
                properties.Volatility,
                properties.DissipationRate,
                properties.Density);
        }

        public static CargoReactionProperties ToDV(this ReactionProperties properties)
        {
            return new CargoReactionProperties(
                properties.Reactivity,
                properties.ReactivityModifierToOthers,
                properties.IgnitionReactivityMin,
                properties.IgnitionReactivityMax,
                properties.CriticalVolumeIgnitionMin,
                properties.CriticalVolumeIgnitionMax,
                properties.ExplosionDelay);
        }

        public static CargoType_v2 ToV2(this CustomCargo cargo)
        {
            var newCargo = ScriptableObject.CreateInstance<CargoType_v2>();

            newCargo.id = cargo.Identifier;
            newCargo.v1 = (CargoType)(++s_tempValue);

            newCargo.localizationKeyFull = cargo.LocalizationKeyFull;
            newCargo.localizationKeyShort = cargo.LocalizationKeyShort;

            newCargo.massPerUnit = cargo.MassPerUnit;
            newCargo.fullDamagePrice = cargo.FullDamagePrice;
            newCargo.environmentDamagePrice = cargo.EnvironmentDamagePrice;

            newCargo.requiredJobLicenses = cargo.Licenses.Select(x => ((JobLicenses)x).ToV2()).ToArray();
            newCargo.loadableCarTypes = Array.Empty<CargoType_v2.LoadableInfo>();

            return newCargo;
        }

        public static List<CargoType> ToCargoTypeGroup(this CustomCargoGroup group, string cargoName)
        {
            List<CargoType> types = new List<CargoType>();

            foreach (var item in group.CargosIds)
            {
                // Find cargo for the group using the id (name).
                if (!Globals.G.Types.cargos.TryFind(x => x.id == item, out var cargo))
                {
                    CCMod.Error($"Cargo '{item}' is missing (cargo group for '{cargoName}')");
                    continue;
                }

                // Don't include it either if there's no wagon to load it.
                if (cargo.loadableCarTypes.Length == 0)
                {
                    CCMod.Error($"Cargo '{item}' cannot be loaded on any car (cargo group for '{cargoName}')");
                    continue;
                }

                types.Add(cargo.v1);
            }

            return types;
        }
    }
}
