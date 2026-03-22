using UnityEditor;
using UnityEngine;

using YeKostenko.CoreKit.Structs;

namespace YeKostenko.CoreKit.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxValue))]
    public class MinMaxValuePropertyEditor : PropertyDrawer
    {
        private const float LabelWidth = 30f;
        private const float Spacing = 5f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            float fieldWidth = (position.width - (LabelWidth * 2) - Spacing) / 2;
            Rect minLabelRect = new Rect(position.x, position.y, LabelWidth, position.height);
            Rect minFieldRect = new Rect(position.x + LabelWidth, position.y, fieldWidth, position.height);
            Rect maxLabelRect = new Rect(position.x + LabelWidth + minFieldRect.width + Spacing,
                position.y, LabelWidth, position.height);
            Rect maxFieldRect = new Rect(position.x + (LabelWidth * 2) + minFieldRect.width + Spacing,
                position.y, minFieldRect.width, position.height);

            // Find the properties
            SerializedProperty minProp = property.FindPropertyRelative("_min");
            SerializedProperty maxProp = property.FindPropertyRelative("_max");

            // Draw fields
            EditorGUI.LabelField(minLabelRect, "Min");
            EditorGUI.PropertyField(minFieldRect, minProp, GUIContent.none);
            EditorGUI.LabelField(maxLabelRect, "Max");
            EditorGUI.PropertyField(maxFieldRect, maxProp, GUIContent.none);

            // Ensure Min is not greater than Max
            if (minProp.floatValue > maxProp.floatValue)
            {
                minProp.floatValue = maxProp.floatValue;
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}