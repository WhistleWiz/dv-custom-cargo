using System.Collections.Generic;
using UnityEngine;

namespace CC.Common
{
    public class CommonCargoObject : ScriptableObject
    {
        public string? Identifier;
        public Sprite? Icon = null;
        public Sprite? ResourceIcon = null;
        public List<ModelsForVanillaCar> Models = new List<ModelsForVanillaCar>();
    }
}
