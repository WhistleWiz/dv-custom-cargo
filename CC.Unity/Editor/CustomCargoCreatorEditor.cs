using CC.Common;
using UnityEditor;
using UnityEngine;

namespace CC.Unity.Editor
{
    [CustomEditor(typeof(CustomCargoCreator))]
    internal class CustomCargoCreatorEditor : UnityEditor.Editor
    {

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

            // Iterate through the properties manually so it doesn't show up as 2 dropdowns.
            SerializedProperty current = _cargo.FindPropertyRelative(nameof(CustomCargo.Identifier));
            EditorGUILayout.PropertyField(current);
            bool modelsExpanded = false;

            while (current.Next(false))
            {
                if (current.name == nameof(CustomCargo.CSVLink))
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.LabelField("Translation", EditorStyles.boldLabel);
                }
                else if (current.name == nameof(CustomCargo.CargoGroups))
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.LabelField("Jobs", EditorStyles.boldLabel);
                }
                else if (current.name == nameof(CustomCargo.Properties))
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.LabelField("Damage and Hazmat", EditorStyles.boldLabel);
                }
                else if (current.name == nameof(CustomCargo.Author))
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.LabelField("Mod Info", EditorStyles.boldLabel);
                }
                else if (current.name == nameof(CustomCargoCreator.Icon))
                {
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.LabelField("Visuals", EditorStyles.boldLabel);
                }

                EditorGUILayout.PropertyField(current);

                if (current.name == nameof(CustomCargo.Properties))
                {
                    if (current.isExpanded)
                    {
                        if (GUILayout.Button("Reset Damage Properties"))
                        {
                            _ccc.Cargo.Properties.ResetDamageProperties();
                            Save();
                        }

                        if (GUILayout.Button("Reset Leak Properties"))
                        {
                            _ccc.Cargo.Properties.LeakProperties = new LeakProperties();
                            Save();
                        }

                        if (GUILayout.Button("Reset Reaction Properties"))
                        {
                            _ccc.Cargo.Properties.ReactionProperties = new ReactionProperties();
                            Save();
                        }
                    }
                }

                modelsExpanded = current.isExpanded;
            }

            if (modelsExpanded)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        TypeForNewSet = (CarParentType)EditorGUILayout.EnumPopup("Car Type", TypeForNewSet);

                        if (GUILayout.Button(s_createAddContent))
                        {
                            _ccc.CreateModelSet(TypeForNewSet);
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Update the object before showing the export button.
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            if (_ccc.DisplayWarning)
            {
                GUI.backgroundColor = EditorHelper.Colours.Warning;
            }

            if (GUILayout.Button(s_exportContent))
            {
                _ccc.ExportBundle();
                // Prevent a weird unity behaviour that is otherwise harmless.
                GUIUtility.ExitGUI();
            }

            GUI.backgroundColor = Color.white;
        }

        private void Save()
        {
            EditorUtility.SetDirty(_ccc);
            AssetDatabase.SaveAssets();
        }
    }
}
