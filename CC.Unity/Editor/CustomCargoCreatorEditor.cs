using UnityEditor;
using UnityEngine;

namespace CC.Unity.Editor
{
    [CustomEditor(typeof(CustomCargoCreator))]
    internal class CustomCargoCreatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("Export"))
            {
                ((CustomCargoCreator)target).ExportModels();
            }
        }
    }
}
