using CC.Common;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using UnityEngine;

namespace CC.Game
{
    internal static class Extensions
    {
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
    }
}
