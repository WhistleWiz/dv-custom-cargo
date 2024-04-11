using CC.Common.Conditions;
using UnityEditor;
using UnityEngine;

namespace CC.Unity.Inspector
{
    [CustomEditor(typeof(ConditionIRLTime))]
    internal class ConditionIRLTimeEditor : Editor
    {
        private SerializedProperty _repeatMonth = null!;
        private SerializedProperty _repeatDay = null!;
        private SerializedProperty _startMonth = null!;
        private SerializedProperty _startDay = null!;
        private SerializedProperty _startHour = null!;
        private SerializedProperty _startMinute = null!;
        private SerializedProperty _endMonth = null!;
        private SerializedProperty _endDay = null!;
        private SerializedProperty _endHour = null!;
        private SerializedProperty _endMinute = null!;

        private void OnEnable()
        {
            _repeatMonth = serializedObject.FindProperty(nameof(ConditionIRLTime.RepeatMonthly));
            _repeatDay = serializedObject.FindProperty(nameof(ConditionIRLTime.RepeatDaily));

            _startMonth = serializedObject.FindProperty(nameof(ConditionIRLTime.StartMonth));
            _startDay = serializedObject.FindProperty(nameof(ConditionIRLTime.StartDay));
            _startHour = serializedObject.FindProperty(nameof(ConditionIRLTime.StartHour));
            _startMinute = serializedObject.FindProperty(nameof(ConditionIRLTime.StartMinute));

            _endMonth = serializedObject.FindProperty(nameof(ConditionIRLTime.EndMonth));
            _endDay = serializedObject.FindProperty(nameof(ConditionIRLTime.EndDay));
            _endHour = serializedObject.FindProperty(nameof(ConditionIRLTime.EndHour));
            _endMinute = serializedObject.FindProperty(nameof(ConditionIRLTime.EndMinute));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_repeatMonth);
            EditorGUILayout.PropertyField(_repeatDay);

            GUI.enabled = !_repeatMonth.boolValue && !_repeatDay.boolValue;

            EditorGUILayout.PropertyField(_startMonth);
            EditorGUILayout.PropertyField(_endMonth);

            GUI.enabled = !_repeatDay.boolValue;

            _startDay.intValue = EditorGUILayout.IntPopup("Start Day", _startDay.intValue, RangeAsString(1, 31), RangeAsInt(1, 31));
            _endDay.intValue = EditorGUILayout.IntPopup("End Day", _endDay.intValue, RangeAsString(1, 31), RangeAsInt(1, 31));

            GUI.enabled = true;

            _startHour.intValue = EditorGUILayout.IntPopup("Start Hour", _startHour.intValue, RangeAsString(0, 23), RangeAsInt(0, 23));
            _endHour.intValue = EditorGUILayout.IntPopup("End Hour", _endHour.intValue, RangeAsString(0, 23), RangeAsInt(0, 23));

            EditorGUILayout.PropertyField(_startMinute);
            EditorGUILayout.PropertyField(_endMinute);
        }

        private static string[] RangeAsString(int min, int max)
        {
            int length = max - min + 1;
            string[] range = new string[length];

            for (int i = 0; i < length; i++)
            {
                range[i] = (i + min).ToString();
            }

            return range;
        }

        private static int[] RangeAsInt(int min, int max)
        {
            int length = max - min + 1;
            int[] range = new int[length];

            for (int i = 0; i < length; i++)
            {
                range[i] = i + min;
            }

            return range;
        }
    }
}
