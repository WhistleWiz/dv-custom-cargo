using DVLangHelper.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CC.Common
{
    [Serializable]
    public class CustomCargo
    {
        [Tooltip("The name (id) of the cargo")]
        public string Identifier = "MyCargoName";
        [Tooltip("Mass per carload\n" +
            "If loading on vanilla cars, use those as a base")]
        public float MassPerUnit = 1000.0f;
        [Tooltip("How much the cargo is worth")]
        public float FullDamagePrice = 10000.0f;
        [Tooltip("The environmental fee when the cargo reaches 100% damage")]
        public float EnvironmentDamagePrice = 0.0f;

        [Tooltip("Link to a CSV file to dynamically load translations (Optional)")]
        public string? CSVLink;
        [Tooltip("These translations are used on job booklets")]
        public TranslationData? TranslationDataFull;
        [Tooltip("These translations are used on the side of cars and the remote")]
        public TranslationData? TranslationDataShort;

        [Tooltip("The cargo groups that this cargo belongs to\n" +
            "A cargo group defines what cargos can be hauled together\n" +
            "You can use vanilla cargos or other custom cargos (use their id)")]
        public CustomCargoGroup[] CargoGroups = new CustomCargoGroup[0];

        [Tooltip("If this cargo can be loaded on a vanilla wagon (Optional)")]
        public CarParentType[] VanillaTypesToLoad = new CarParentType[0];

        [Tooltip("The licenses required to haul this cargo")]
        public string[] Licenses = new string[0];

        public CargoHazmatProperties Properties = new CargoHazmatProperties();

        [Tooltip("Your name, probably")]
        public string Author = string.Empty;
        public string Version = "1.0.0";
        [Tooltip("Add a link to your website here (Optional)")]
        public string? HomePage;
        [Tooltip("Add a repository link here to be able to automatically update your cargo (Optional)")]
        public string? Repository;
        [Tooltip("If you have included CCL components in the models, use this checkbox to add CCL to the mod requirements")]
        public bool RequireCCL = false;

        public string LocalizationKeyFull => $"{Constants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}";
        public string LocalizationKeyShort => $"{Constants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}_short";

        public string[] GetRequirements()
        {
            List<string> reqs = new List<string> { Constants.MainModId };

            if (RequireCCL)
            {
                reqs.Add(Constants.CCL);
            }

            return reqs.ToArray();
        }
    }
}
