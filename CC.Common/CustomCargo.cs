using DVLangHelper.Data;
using Newtonsoft.Json;
using System;
using System.Security.Policy;
using UnityEngine;

namespace CC.Common
{
    [Serializable]
    public class CustomCargo
    {
        public const int CUSTOM_TYPE_OFFSET = 0x4000_0000;
        public const int CUSTOM_TYPE_MASK = CUSTOM_TYPE_OFFSET - 1;

        [Tooltip("The name (id) of the cargo")]
        public string Identifier = "MyCargoName";
        [JsonIgnore, Tooltip("Check this to allow overriding the cargo's internal value\n" +
            "DO NOT  override unless there's a conflict with another mod")]
        public bool OverrideValue = false;
        [Tooltip("The internal value of the cargo\n" +
            "DO NOT modify it manually unless there's a conflict with another mod")]
        public int Value = 10000;
        [Tooltip("Mass per carload\n" +
            "If loading on vanilla cars, use those as a base")]
        public float MassPerUnit = 1000.0f;
        public float FullDamagePrice = 10000.0f;
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

        [Tooltip("Your name, probably")]
        public string Author = string.Empty;
        public string Version = "1.0.0";
        [Tooltip("Add a link to your website here (Optional)")]
        public string? HomePage;
        [Tooltip("Add a repository link here to be able to automatically update your cargo (Optional)")]
        public string? Repository;

        public string LocalizationKeyFull => $"{NameConstants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}";
        public string LocalizationKeyShort => $"{NameConstants.LocalizeRoot}/{Identifier.Replace(" ", "_").ToLowerInvariant()}_short";

        public CustomCargoGroup GetDefaultCargoGroup()
        {
            return new CustomCargoGroup(Identifier);
        }

        public void GenerateId()
        {
            Value = Mathf.Max(10000, GenerateId(Identifier));
        }

        public static int GenerateId(string name)
        {
            // Really? Really.
            // Unfortunately it's a bit hard to get unique ids for everyone
            // when its not known what other mods have, so this is the best
            // we can do for now. Otherwise, you can override the generated
            // value in case there's a known collision.
            // https://github.com/derail-valley-modding/custom-car-loader/blob/v1.8.4/DVCustomCarLoader/BaseInjector.cs#L15
            int hash = name.GetHashCode();
            return (hash & CUSTOM_TYPE_MASK) + CUSTOM_TYPE_OFFSET;
        }
    }
}
