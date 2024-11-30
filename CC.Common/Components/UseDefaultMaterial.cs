using UnityEngine;

namespace CC.Common.Components
{
    [RequireComponent(typeof(Renderer))]
    public class UseDefaultMaterial : MonoBehaviour
    {
        [Tooltip("One of the default materials available ingame")]
        public string MaterialName = "";

        public Renderer Renderer => GetComponent<Renderer>();
    }
}
