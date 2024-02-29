using CC.Common.Effects;
using System;
using UnityEngine;

namespace CC.Common
{
    [CreateAssetMenu(menuName = "DVCustomCargo/Cargo Damage Properties")]
    public class CargoDamageProperties : ScriptableObject
    {
        [Tooltip("The reaction effects of this cargo with other things")]
        public CargoEffectPools CargoEffectPools = CargoEffectPools.None;
        [Tooltip("How this cargo leaks")]
        public CargoLeakCurve LeakCurve = CargoLeakCurve.None;
        [Tooltip("How the reaction happens")]
        public HazmatReaction Reaction = HazmatReaction.None;
        [Tooltip("The reaction curve if set to custom")]
        public AnimationCurve CustomReactionCurve = new AnimationCurve();

        private void OnValidate()
        {
            // Force cargo pools to be in the valid options.
            CargoEffectPools cep = CargoEffectPools.None;

            foreach (CargoEffectPools item in Enum.GetValues(typeof(CargoEffectPools)))
            {
                if (CargoEffectPools.HasFlag(item))
                {
                    cep |= item;
                }
            }

            CargoEffectPools = cep;

            // If leak curve is none, force it to be liquid or gas if the cargo
            // is in certain pools.
            if (LeakCurve == CargoLeakCurve.None)
            {
                if (CargoEffectPools.HasFlag(CargoEffectPools.Gases) ||
                    CargoEffectPools.HasFlag(CargoEffectPools.FlammableGases) ||
                    CargoEffectPools.HasFlag(CargoEffectPools.ExtiguishingGases))
                {
                    LeakCurve = CargoLeakCurve.Gas;
                }

                if (CargoEffectPools.HasFlag(CargoEffectPools.Oils) ||
                    CargoEffectPools.HasFlag(CargoEffectPools.Liquids) ||
                    CargoEffectPools.HasFlag(CargoEffectPools.FlammableLiquids) ||
                    CargoEffectPools.HasFlag(CargoEffectPools.CorrosiveLiquids))
                {
                    LeakCurve = CargoLeakCurve.Liquid;
                }
            }
        }
    }
}
