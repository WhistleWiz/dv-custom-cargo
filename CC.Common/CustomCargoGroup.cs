﻿using System;
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
        [Tooltip("The station IDs that can generate this cargo (i.e. OWN, HB, MFMB)\n" +
            "IDs from custom maps can also be used\n" +
            "You can use full track notation (HB-D7L, FM-A2L) to use specific tracks for loading")]
        public string[] SourceStations = new string[0];
        [Tooltip("The station IDs that can receive this cargo")]
        public string[] DestinationStations = new string[0];

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
