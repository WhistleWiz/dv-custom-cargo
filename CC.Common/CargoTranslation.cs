using DVLangHelper.Data;

namespace CC.Common
{
    public class CargoTranslation
    {
        public DVLanguage Language;
        public string Full;
        public string Short;

        public CargoTranslation(DVLanguage language, string f, string s)
        {
            Language = language;
            Full = f;
            Short = s;
        }
    }
}
