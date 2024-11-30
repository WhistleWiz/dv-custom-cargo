using CC.Common;
using CC.Common.Components;
using UnityEditor;
using UnityEngine;

namespace CC.Unity.Inspector
{
    [CustomEditor(typeof(UseDefaultMaterial))]
    internal class UseDefaultMaterialEditor : Editor
    {
        private UseDefaultMaterial _comp = null!;
        private SerializedProperty _name = null!;
        private int _popup = 0;

        private void OnEnable()
        {
            _name = serializedObject.FindProperty(nameof(UseDefaultMaterial.MaterialName));
        }

        public override void OnInspectorGUI()
        {
            _comp = (UseDefaultMaterial)target;

            EditorGUILayout.PropertyField(_name);

            EditorGUILayout.BeginHorizontal();

            _popup = EditorGUILayout.Popup(_popup, Constants.MaterialNames);

            if (GUILayout.Button("Set", GUILayout.ExpandWidth(false)))
            {
                _name.stringValue = Constants.MaterialNames[_popup];
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
