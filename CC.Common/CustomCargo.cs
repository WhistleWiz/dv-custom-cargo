using DVLangHelper.Data;
using Newtonsoft.Json;

namespace CC.Common
{
    public class CustomCargo
    {
        [JsonIgnore]
        public int Id = 10000;
        public string Name = "MyCargoName";
        public float MassPerUnit = 1000.0f;
        public float FullDamagePrice = 10000.0f;
        public float EnvironmentDamagePrice = 0.0f;
        public BaseGameLicense[] Licenses = new[] { BaseGameLicense.Basic };

        public string? Csv;
        public TranslationData? TranslationDataFull;
        public TranslationData? TranslationDataShort;

        public string[] SourceStations = new string[0];
        public string[] DestinationStations = new string[0];

        public CargoGroup[] CargoGroups = new CargoGroup[0];

        public string Author = string.Empty;

        public string LocalizationKeyFull => $"{NameConstants.LocalizeRoot}/{Name.Replace(" ", "_").ToLowerInvariant()}";
        public string LocalizationKeyShort => $"{NameConstants.LocalizeRoot}/{Name.Replace(" ", "_").ToLowerInvariant()}_short";

        public CargoGroup GetDefaultCargoGroup()
        {
            return new CargoGroup(Name);
        }
    }
}
