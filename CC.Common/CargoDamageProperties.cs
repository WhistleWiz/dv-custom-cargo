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
            if (!Enum.IsDefined(typeof(CargoEffectPools), CargoEffectPools))
            {
                CargoEffectPools = CargoEffectPools.None;
            }
        }
    }
}
