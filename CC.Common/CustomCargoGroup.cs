namespace CC.Common
{
    public class CustomCargoGroup
    {
        public string[] CargosIds;

        public CustomCargoGroup(params string[] cargos)
        {
            CargosIds = cargos;
        }
    }
}
