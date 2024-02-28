using DVLangHelper.Data;
using System;
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
        [Tooltip("The licenses required to haul this cargo")]
        public BaseGameLicense[] Licenses = new[] { BaseGameLicense.Basic };

        [Tooltip("Link to a CSV file to dynamically load translations (Optional)")]
        public string? CSVLink;
        [Tooltip("These translations are used on job booklets")]
        public TranslationData? TranslationDataFull;
        [Tooltip("These translations are used on the side of cars and the remote")]
        public TranslationData? TranslationDataShort;

        [Tooltip("The station IDs that can generate this cargo (i.e. OWN, HB, MFMB)\n" +
            "IDs from custom maps can also be used")]
        public string[] SourceStations = new string[0];
        [Tooltip("The station IDs that can receive this cargo")]
        public string[] DestinationStations = new string[0];

        [Tooltip("The cargo groups that this cargo belongs to\n" +
            "A cargo group defines what cargos can be hauled together\n" +
            "You can use vanilla cargos or other custom cargos (use their id)")]
        public CustomCargoGroup[] CargoGroups = new CustomCargoGroup[0];

        [Tooltip("If this cargo can be loaded on a vanilla wagon (Optional)")]
        public CarParentType[] VanillaTypesToLoad = new CarParentType[0];

        public CargoDamageProperties Properties = null!;

        [Tooltip("Your name, probably")]
        public string Author = string.Empty;
        public string Version = "1.0.0";
        [Tooltip("Add a link to your website here (Optional)")]
        public string? HomePage;
        [Tooltip("Add a repository link here to be able to automatically update your cargo (Optional)")]
        public string? Repository;

        public string LocalizationKeyFull => $"{Constants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}";
        public string LocalizationKeyShort => $"{Constants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}_short";
    }
}
