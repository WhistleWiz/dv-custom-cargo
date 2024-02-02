using System.Linq;

namespace CC.Common
{
    public class CargoPrefab
    {
        private static CargoPrefab[] s_all = new[]
        {
            new CargoPrefab("Container AAG",                            906, 0, 0),
            new CargoPrefab("Container Brohm",                          905, 0, 0),
            new CargoPrefab("Container Chemlek AC",                     910, 0, 0),
            new CargoPrefab("Container Goorsk",                         903, 0, 0),
            new CargoPrefab("Container Iskar",                          901, 0, 0),
            new CargoPrefab("Container Krugmann",                       904, 0, 0),
            new CargoPrefab("Container NeoGamma",                       911, 0, 0),
            new CargoPrefab("Container Novae",                          908, 0, 0),
            new CargoPrefab("Container Novae Old",                      908, 0, 1),
            new CargoPrefab("Container Obco",                           902, 0, 0),
            new CargoPrefab("Container Red",                            905, 0, 1),
            new CargoPrefab("Container Sperex",                         907, 0, 0),
            new CargoPrefab("Container SunOmni",                        900, 0, 0),
            new CargoPrefab("Container SunOmni AC",                     900, 0, 1),
            new CargoPrefab("Container Traeg",                          909, 0, 0),
            new CargoPrefab("Container White",                          900, 0, 2),
            new CargoPrefab("Container White AC",                       900, 0, 3),

            new CargoPrefab("Container Acetylene (Medium x2)",          281, 0, 0),
            new CargoPrefab("ISO Tank Yellow Asphyxiating (x2)",        284, 0, 0),
            new CargoPrefab("ISO Tank Yellow Explosive (x2)",           283, 0, 0),
            new CargoPrefab("ISO Tank Yellow Oxydizing (x2)",           282, 0, 0),
            new CargoPrefab("Nuclear Flask Container",                  300, 0, 0),

            new CargoPrefab("Container Military",                       322, 0, 0),
            new CargoPrefab("Container Military (Habitat)",             322, 0, 1),
            new CargoPrefab("Container Military (Medium)",              322, 0, 4),
            new CargoPrefab("Container Military (Medium x2)",           322, 0, 5),
            new CargoPrefab("Container Military (Medium, Habitat)",     322, 0, 2),
            new CargoPrefab("Container Military (Medium, Habitat x2)",  322, 0, 3),
            new CargoPrefab("Container Military (Small x3)",            302, 0, 0),

            new CargoPrefab("Explosive Labels (Boxcar)",                301, 0, 0),
            new CargoPrefab("Corrosive Labels (Tanker)",                301, 0, 0),
            new CargoPrefab("Explosive Labels (Tanker)",                280, 0, 0),
            new CargoPrefab("Flammable Labels (Tanker)",                 40, 0, 0),
            new CargoPrefab("Toxic and Corrosive Labels (Tanker)",      286, 0, 0),
        };

        public static CargoPrefab[] All => s_all;

        private static string[] s_names = All.Select(x => x.DisplayName).ToArray();

        public static string[] Names => s_names;

        public string DisplayName;
        public int TypeEnum;
        public int CarIndex;
        public int VariantIndex;

        private CargoPrefab(string displayName, int typeEnum, int carIndex, int variantIndex)
        {
            DisplayName = displayName;
            TypeEnum = typeEnum;
            CarIndex = carIndex;
            VariantIndex = variantIndex;
        }
    }
}
