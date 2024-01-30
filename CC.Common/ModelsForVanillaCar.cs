using UnityEngine;

namespace CC.Common
{
    public class ModelsForVanillaCar : ScriptableObject
    {
        public CarParentType CarType;
        public GameObject[] Prefabs = new GameObject[0];
    }
}
