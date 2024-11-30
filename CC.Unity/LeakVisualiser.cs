using UnityEditor;
using UnityEngine;

namespace CC.Unity
{
    internal class LeakVisualiser : MonoBehaviour
    {
        private void Awake()
        {
            Debug.LogWarning($"You should delete the leak visualiser from the prefab '{transform.root.gameObject}'!");
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            foreach (Transform t in transform)
            {
                Gizmos.DrawLine(t.position, t.position + t.right);
                Handles.DrawSolidArc(t.position, t.right, t.forward, 360, 0.1f);
            }

            Gizmos.color = Color.white;
        }
    }
}
