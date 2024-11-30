using CC.Common.Components;
using CC.Common;
using UnityEditor;
using UnityEngine;

namespace CC.Unity.Inspector
{
    [CustomEditor(typeof(UseDefaultMesh))]
    internal class UseDefaultMeshEditor : Editor
    {
        private UseDefaultMesh _comp = null!;
        private SerializedProperty _name = null!;
        private int _popup = 0;

        private void OnEnable()
        {
            _name = serializedObject.FindProperty(nameof(UseDefaultMesh.MeshName));
        }

        public override void OnInspectorGUI()
        {
            _comp = (UseDefaultMesh)target;

            EditorGUILayout.PropertyField(_name);

            EditorGUILayout.BeginHorizontal();

            _popup = EditorGUILayout.Popup(_popup, Constants.MeshNames);

            if (GUILayout.Button("Set", GUILayout.ExpandWidth(false)))
            {
                _name.stringValue = Constants.MeshNames[_popup];
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
