using UnityEngine;

namespace CC.Common
{
    public static class Helper
    {
        public static bool IsVanillaStation(string id)
        {
            switch (id)
            {
                case "OWC":
                case "FRS":
                case "MFMB":
                case "CSW":
                case "IME":
                case "MF":
                case "FF":
                case "HMB":
                case "HB":
                case "MB":
                case "CM":
                case "GF":
                case "FM":
                case "SM":
                case "SW":
                case "IMW":
                case "OWN":
                case "FRC":
                    return true;
                default:
                    return false;
            }
        }
    }
}
