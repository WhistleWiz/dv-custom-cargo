using CC.Common;
using UnityEditor;
using UnityEngine;

namespace CC.Unity.Editor
{
    [CustomEditor(typeof(CustomCargoCreator))]
    internal class CustomCargoCreatorEditor : UnityEditor.Editor
    {
        public static readonly Color Warning = new Color(2.00f, 1.50f, 0.25f);

        private static GUIContent s_createAddContent = new GUIContent("Create model set",
            "Creates and automatically assigns a model set for this cargo");
        private static GUIContent s_exportContent = new GUIContent("Export",
            "Exports your cargo to a zip file you can drag and drop into UMM\n" +
            "You can also extract is manually into the Mods folder if you want");

        public CarParentType TypeForNewSet = CarParentType.None;

        private CustomCargoCreator _ccc = null!;
        private SerializedProperty _cargo = null!;

        private void OnEnable()
        {
            _cargo = serializedObject.FindProperty(nameof(CustomCargoCreator.Cargo));
        }

        public override void OnInspectorGUI()
        {
            _ccc = (CustomCargoCreator)target;

            // Iterate through the properties manually so we can have custom behaviour.
            SerializedProperty current = _cargo.FindPropertyRelative(nameof(CustomCargo.Identifier));

            EditorGUILayout.PropertyField(current);

            current.Next(false);

            // Don't show the internal IDs or the option to override them to the user for now.
            //if (current.boolValue)
            //{
            //    GUI.backgroundColor = Warning;
            //}

            //EditorGUILayout.PropertyField(current);
            //GUI.enabled = current.boolValue;

            //current.Next(false);
            //current.intValue = _ccc.Cargo.Value;
            //EditorGUILayout.PropertyField(current);
            //GUI.enabled = true;
            //GUI.backgroundColor = Color.white;

            current.Next(false);

            while (current.Next(false))
            {
                EditorGUILayout.PropertyField(current);
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            TypeForNewSet = (CarParentType)EditorGUILayout.EnumPopup("Car Type", TypeForNewSet);

            if (GUILayout.Button(s_createAddContent))
            {
                _ccc.CreateModelSet(TypeForNewSet);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (_ccc.DisplayWarning)
            {
                GUI.backgroundColor = Warning;
            }

            if (GUILayout.Button(s_exportContent))
            {
                _ccc.ExportModels();
            }

            GUI.backgroundColor = Color.white;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
