using UnityEditor;
using UnityEngine;

namespace CC.Unity.Editor
{
    [CustomEditor(typeof(CustomCargoPack))]
    internal class CustomCargoPackEditor : UnityEditor.Editor
    {
        private static GUIContent s_exportContent = new GUIContent("Export",
            "Exports your cargo pack to a zip file you can drag and drop into UMM\n" +
            "You can also extract is manually into the Mods folder if you want");

        private CustomCargoPack _ccp = null!;

        public override void OnInspectorGUI()
        {
            _ccp = (CustomCargoPack)target;

            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (_ccp.Result.Failed)
            {
                GUI.backgroundColor = EditorHelper.Colours.Cancel;
            }
            else if (_ccp.Result.RequireConfirm)
            {
                GUI.backgroundColor = EditorHelper.Colours.Warning;
            }

            if (GUILayout.Button(s_exportContent))
            {
                _ccp.Export();
                // Prevent a weird unity behaviour that is otherwise harmless.
                GUIUtility.ExitGUI();
            }

            GUI.backgroundColor = Color.white;
        }
    }
}
