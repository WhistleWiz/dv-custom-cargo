using CC.Common;
using UnityEditor;

namespace CC.Unity.Editor
{
    [CustomEditor(typeof(UseCargoPrefab))]
    internal class UseCargoPrefabEditor : UnityEditor.Editor
    {
        private UseCargoPrefab _ucp = null!;
        private SerializedProperty _prefabIndex = null!;

        private void OnEnable()
        {
            _prefabIndex = serializedObject.FindProperty(nameof(UseCargoPrefab.PrefabIndex));
        }

        public override void OnInspectorGUI()
        {
            _ucp = (UseCargoPrefab)target;

            _prefabIndex.intValue = EditorGUILayout.Popup("Prefab To Use", _prefabIndex.intValue, CargoPrefab.Names);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
