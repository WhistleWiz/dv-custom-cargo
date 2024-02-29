using CC.Common.Effects;
using System;
using UnityEngine;

namespace CC.Common
{
    [Serializable]
    public class CargoHazmatProperties
    {
        public DamageProperties DamageProperties = new DamageProperties();

        [Tooltip("The reaction effects of this cargo with other things")]
        public CargoEffectPools CargoEffectPools = CargoEffectPools.None;
        [Space]
        [Tooltip("How this cargo leaks")]
        public CargoLeakCurve LeakCurve = CargoLeakCurve.None;
        public LeakProperties LeakProperties = new LeakProperties();
        [Space]
        [Tooltip("How the reaction happens")]
        public HazmatReaction Reaction = HazmatReaction.None;
        [Tooltip("The reaction curve if set to custom")]
        public AnimationCurve CustomReactionCurve = new AnimationCurve();
        public ReactionProperties ReactionProperties = new ReactionProperties();

        public void OnValidate()
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

    [Serializable]
    public class DamageProperties
    {
        public float MaxHealth;
        public float DamageTolerance;
        public float DamageMultiplier;
        public float DamageResistance;
        public float FireDamageMultiplier;
        public float FireResistance;

        public DamageProperties()
        {
            MaxHealth = 8000f;
            DamageTolerance = 0.01f;
            DamageMultiplier = 1f;
            DamageResistance = 50.0f;
            FireDamageMultiplier = 1f;
            FireResistance = 7.5f;
        }
    }

    [Serializable]
    public class LeakProperties
    {
        public float MaxLeakFlow;
        public float MinLeakFlow;
        public float Volatility;
        public float DissipationRate;
        public float Density;

        public LeakProperties()
        {
            MaxLeakFlow = 500.0f;
            MinLeakFlow = 100.0f;
            Volatility = 0.0f;
            DissipationRate = 0.0f;
            Density = 0.0f;
        }
    }

    [Serializable]
    public class ReactionProperties
    {
        public float Reactivity;
        public float ReactivityModifierToOthers;
        public float IgnitionReactivityMin;
        public float IgnitionReactivityMax;
        public float CriticalVolumeIgnitionMin;
        public float CriticalVolumeIgnitionMax;
        public float ExplosionDelay;

        public ReactionProperties()
        {
            Reactivity = -5.0f;
            ReactivityModifierToOthers = 0.0f;
            IgnitionReactivityMin = float.PositiveInfinity;
            IgnitionReactivityMax = float.PositiveInfinity;
            CriticalVolumeIgnitionMin = float.PositiveInfinity;
            CriticalVolumeIgnitionMax = float.PositiveInfinity;
            ExplosionDelay = 0.0f;
        }
    }
}
