using CC.Common;
using System.Linq;

namespace CC.Unity.Validation
{
    internal class CargoValidator
    {
        public static ValidationResult ValidateCargo(CustomCargoCreator cargo)
        {
            ValidationResult result = new ValidationResult();
            CustomCargo c = cargo.Cargo;

            if (c.TranslationDataFull != null &&
                c.TranslationDataFull.Items.Any(x => string.IsNullOrEmpty(x.Value)))
            {
                result.ScaleToWarning($"{cargo.name} - Translation Data Full has empty translations!");
            }

            if (c.TranslationDataShort != null &&
                c.TranslationDataShort.Items.Any(x => string.IsNullOrEmpty(x.Value)))
            {
                result.ScaleToWarning($"{cargo.name} - Translation Data Short has empty translations!");
            }

            foreach (var group in c.CargoGroups)
            {
                if (group.SourceStations.Length < 1)
                {
                    result.ScaleToWarning($"{cargo.name} - Cargo group has no source stations!");
                }

                if (group.DestinationStations.Length < 1)
                {
                    result.ScaleToWarning($"{cargo.name} - Cargo group has no destination stations!");
                }

                if (group.SourceStations.Any(x => group.DestinationStations.Contains(x)) ||
                    group.DestinationStations.Any(x => group.SourceStations.Contains(x)))
                {
                    result.ScaleToWarning($"{cargo.name} - Cargo group has the same station in both sources and destinations!");
                }
            }

            foreach (var model in cargo.Models)
            {
                if (!c.VanillaTypesToLoad.Contains(model.CarType))
                {
                    result.ScaleToWarning($"{cargo.name} - Car type {model.CarType} is not setup to be loaded!");
                }

                foreach (var prefab in model.Prefabs)
                {
                    if (!prefab.GetComponent<UseCargoPrefab>())
                    {
                        if (prefab.GetComponentInChildren<UseCargoPrefab>())
                        {
                            result.ScaleToFailure($"{prefab.name} - Found UseCargoPrefab but it's not in the root object!");
                        }

                        if (!prefab.transform.Find(Constants.CollidersRoot))
                        {
                            result.ScaleToFailure($"{prefab.name} - Missing [colliders] child!");
                        }
                        else if (!prefab.transform.Find(Constants.CollidersCollision))
                        {
                            result.ScaleToFailure($"{prefab.name} - Missing [collision] child in [colliders]!");
                        }
                    }
                }
            }

            return result;
        }
    }
}
