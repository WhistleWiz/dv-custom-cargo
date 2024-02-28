using DV.ThingTypes;
using HarmonyLib;
using System;
using System.Linq;

namespace CC.Game.Patches
{
    [HarmonyPatch(typeof(Enum))]
    internal static class EnumPatches
    {
        // Thanks Passenger Jobs mod!

        // Extend the array of actual values with the ones added by the mod.
        [HarmonyPostfix, HarmonyPatch(nameof(Enum.GetValues))]
        public static void GetValuesPostfix(Type enumType, ref Array __result)
        {
            if (enumType == typeof(CargoType))
            {
                __result = ExtendArray(__result, CargoManager.AddedValues.ToArray());
            }
        }

        private static Array ExtendArray<T>(Array source, params T[] newValues)
        {
            var result = Array.CreateInstance(typeof(T), source.Length + newValues.Length);
            Array.Copy(source, result, source.Length);
            Array.Copy(newValues, 0, result, source.Length, newValues.Length);
            return result;
        }

        // Consider values defined by the mod as valid enum values.
        [HarmonyPrefix, HarmonyPatch(nameof(Enum.IsDefined))]
        public static bool IsDefinedPrefix(Type enumType, object value, ref bool __result)
        {
            if (enumType == typeof(CargoType))
            {
                if (value is int iVal && CargoManager.AddedValues.Contains((CargoType)iVal) ||
                    value is CargoType cVal && CargoManager.AddedValues.Contains(cVal))
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}
