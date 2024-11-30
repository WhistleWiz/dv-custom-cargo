using UnityEngine;

namespace CC.Common.Components
{
    [RequireComponent(typeof(MeshFilter))]
    public class UseDefaultMesh : MonoBehaviour
    {
        [Tooltip("One of the default meshes available ingame")]
        public string MeshName = "";

        public MeshFilter MeshFilter => GetComponent<MeshFilter>();
    }
}
