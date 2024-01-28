namespace CC.Common
{
    public class CustomCargo
    {
        public string Name = "MyCargoName";
        public int Id = 10000;
        public string LocalizationKeyFull = "mycargonamefull";
        public string LocalizationKeyShort = "mycargonameshort";
        public float MassPerUnit = 1000.0f;
        public float FullDamagePrice = 10000.0f;
        public float EnvironmentDamagePrice = 0.0f;
        public BaseGameLicense[] Licenses = new[] { BaseGameLicense.Basic };

        public string[] SourceStations = new string[0];
        public string[] DestinationStations = new string[0];

        public CargoGroup[] CargoGroups = new CargoGroup[0];

        public string Author = string.Empty;

        public CargoGroup GetDefaultCargoGroup()
        {
            return new CargoGroup(Name);
        }
    }
}
