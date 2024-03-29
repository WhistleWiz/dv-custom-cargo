﻿using CC.Common;
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

            foreach (var license in c.Licenses)
            {
                if (string.IsNullOrEmpty(license))
                {
                    result.ScaleToFailure($"{cargo.name} - Licenses array has empty values!");
                    break;
                }
            }

            foreach (var group in c.CargoGroups)
            {
                bool flag = false;

                if (group.SourceStations.Length < 1)
                {
                    result.ScaleToWarning($"{cargo.name} - Cargo group has no source stations!");
                    flag = true;
                }

                if (group.DestinationStations.Length < 1)
                {
                    result.ScaleToWarning($"{cargo.name} - Cargo group has no destination stations!");
                    flag = true;
                }

                if (group.SourceStations.Any(x => group.DestinationStations.Contains(x)) ||
                    group.DestinationStations.Any(x => group.SourceStations.Contains(x)))
                {
                    result.ScaleToWarning($"{cargo.name} - Cargo group has the same station in both sources and destinations!");
                    flag = true;
                }

                if (flag)
                {
                    break;
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
