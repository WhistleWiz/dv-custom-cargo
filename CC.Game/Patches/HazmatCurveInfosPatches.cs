using DV.ThingTypes;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace CC.Game.Patches
{
    [HarmonyPatch(typeof(HazmatCurveInfos))]
    internal class HazmatCurveInfosPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(HazmatCurveInfos.GetLeakAndReactionCurves))]
        public static void GetLeakAndReactionCurvesPostfix(CargoType cargoType,
            ref (AnimationCurveAsset leakCurve, AnimationCurveAsset reactionCurve) __result)
        {
            // Do nothing if this was not a cargo added by the mod.
            if (!CargoManager.AddedValues.Contains(cargoType))
            {
                return;
            }

            var cc = CargoManager.AddedCargos.First(x => x.V2.v1 == cargoType).Custom;

            if (cc.Properties.LeakCurve == Common.Effects.CargoLeakCurve.None)
            {
                CCMod.Error($"Cargo {cc.Identifier} has wrong leak type!");
                return;
            }

            __result.leakCurve = HazmatCurvesReferences.HazmatCurveInfos.curveInfos[(int)cc.Properties.LeakCurve].leakCurve;

            if (cc.Properties.Reaction == Common.Effects.HazmatReaction.Custom)
            {
                __result.reactionCurve = ScriptableObject.CreateInstance<AnimationCurveAsset>();
                __result.reactionCurve.curve = cc.Properties.CustomReactionCurve;
            }
            else
            {
                __result.reactionCurve = HazmatCurvesReferences.HazmatCurveInfos.curveInfos[(int)cc.Properties.Reaction].reactionCurve;
            }
        }
    }
}
