using System;
using System.Collections.Generic;
using System.Linq;

namespace CC.Common
{
    [Serializable]
    public class CustomCargoGroup
    {
        public List<string> CargosIds;

        public CustomCargoGroup(params string[] cargos)
        {
            CargosIds = cargos.ToList();
        }

        public void AddIdIfMissing(string id)
        {
            if (!CargosIds.Contains(id))
            {
                CargosIds.Add(id);
            }
        }
    }
}
