using UnityEngine;

namespace CC.Unity.Editor
{
    internal class EditorHelper
    {
        public class Colours
        {
            public static readonly Color Accept = new Color(0.50f, 1.80f, 0.75f);
            public static readonly Color Warning = new Color(2.00f, 1.50f, 0.25f);
            public static readonly Color Cancel = new Color(2.00f, 0.75f, 0.75f);
        }

        public static void DrawCubeGizmoOutlined(Vector3 position, Vector3 size)
        {
            Color c = Gizmos.color;
            Gizmos.color = new Color(c.r, c.g, c.b, c.a * 0.3f);
            Gizmos.DrawCube(position, size);
            Gizmos.color = c;
            Gizmos.DrawWireCube(position, size);
        }
    }
}
