﻿using UnityEngine;

namespace CC.Common
{
    [CreateAssetMenu(menuName = "DVCustomCargo/Model Collection")]
    public class ModelsForVanillaCar : ScriptableObject
    {
        public CarParentType CarType;
        public GameObject[] Prefabs = new GameObject[0];

        public string GetAssetAdressable(string mainId) => $"{mainId}_{CarType}";
    }
}
