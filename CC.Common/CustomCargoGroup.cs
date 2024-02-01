using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CC.Common
{
    [Serializable]
    public class CustomCargoGroup
    {
        [Delayed]
        public List<string> CargosIds;

        public CustomCargoGroup()
        {
            CargosIds = new List<string>();
        }

        public CustomCargoGroup(params string[] cargos)
        {
            CargosIds = cargos.ToList();
        }

        public void AddIdIfMissing(string id)
        {
            if (!CargosIds.Contains(id))
            {
                CargosIds.Insert(0, id);
            }
        }
    }
}
