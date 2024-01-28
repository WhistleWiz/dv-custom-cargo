namespace CC.Common
{
    public class CargoGroup
    {
        public string[] CargosIds;

        public CargoGroup(params string[] cargos)
        {
            CargosIds = cargos;
        }
    }
}
