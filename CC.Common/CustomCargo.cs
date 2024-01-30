﻿using DVLangHelper.Data;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CC.Common
{
    [Serializable]
    public class CustomCargo
    {
        [JsonIgnore, HideInInspector]
        public int Id = 10000;
        [Tooltip("The name (id) of the cargo")]
        public string Name = "MyCargoName";
        [Tooltip("Mass per carload\n" +
            "If loading on vanilla cars, use those as a base")]
        public float MassPerUnit = 1000.0f;
        public float FullDamagePrice = 10000.0f;
        public float EnvironmentDamagePrice = 0.0f;
        [Tooltip("The licenses required to haul this cargo")]
        public BaseGameLicense[] Licenses = new[] { BaseGameLicense.Basic };

        [Tooltip("Optional link to a CSV file to dynamically load translations")]
        public string? Csv;
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
            "You can use vanilla cargos or other custom cargos")]
        public CustomCargoGroup[] CargoGroups = new CustomCargoGroup[0];

        [Tooltip("If this cargo can be loaded on a vanilla wagon (Optional)")]
        public CarParentType[] VanillaTypesToLoad = new CarParentType[0];

        public string Author = string.Empty;
        public string Version = "1.0.0";

        public string LocalizationKeyFull => $"{NameConstants.LocalizeRoot}/{Name.Replace(" ", "_").ToLowerInvariant()}";
        public string LocalizationKeyShort => $"{NameConstants.LocalizeRoot}/{Name.Replace(" ", "_").ToLowerInvariant()}_short";

        public CustomCargoGroup GetDefaultCargoGroup()
        {
            return new CustomCargoGroup(Name);
        }
    }
}