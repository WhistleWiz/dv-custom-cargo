using CC.Common;
using System.Linq;
using UnityEngine;

namespace CC.Unity.Validation
{
    internal class CargoValidator
    {
        public static bool ValidateCargo(CustomCargoCreator cargo)
        {
            CustomCargo c = cargo.Cargo;

            if (c.TranslationDataFull != null &&
                c.TranslationDataFull.Items.Any(x => string.IsNullOrEmpty(x.Value)))
            {
                Debug.LogWarning("Translation Data Full has empty translations!");
                return false;
            }

            if (c.TranslationDataShort != null &&
                c.TranslationDataShort.Items.Any(x => string.IsNullOrEmpty(x.Value)))
            {
                Debug.LogWarning("Translation Data Short has empty translations!");
                return false;
            }

            if (c.SourceStations.Length < 1)
            {
                Debug.LogWarning("Cargo has no source stations!");
                return false;
            }

            if (c.DestinationStations.Length < 1)
            {
                Debug.LogWarning("Cargo has no destination stations!");
                return false;
            }

            if (cargo.Models.Any(x => !c.VanillaTypesToLoad.Contains(x.CarType)))
            {
                Debug.LogWarning("There's a cargo model that won't be loaded into a wagon!");
                return false;
            }

            return true;
        }
    }
}
