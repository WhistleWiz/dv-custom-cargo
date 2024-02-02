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
    }
}
