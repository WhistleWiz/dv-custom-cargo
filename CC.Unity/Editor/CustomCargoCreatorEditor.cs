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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            TypeForNewSet = (CarParentType)EditorGUILayout.EnumPopup("Car Type", TypeForNewSet);

            if (GUILayout.Button(s_createAddContent))
            {
                ((CustomCargoCreator)target).CreateModelSet(TypeForNewSet);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button(s_exportContent))
            {
                ((CustomCargoCreator)target).ExportModels();
            }
        }
    }
}
