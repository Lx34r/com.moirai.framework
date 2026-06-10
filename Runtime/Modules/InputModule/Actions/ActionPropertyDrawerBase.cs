#if UNITY_EDITOR
using UnityEngine;

namespace Moirai.Atropos.Input
{
    /// <summary>
    /// Base property drawer that provides the common boilerplate for input action editors.
    /// All action editors (BoolAction, FloatAction, Vector2Action) share the same pattern:
    /// find the m_Value property, draw a label, then draw the value field.
    /// </summary>
    public abstract class ActionPropertyDrawerBase : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginProperty(position, label, property);

            UnityEditor.SerializedProperty value = property.FindPropertyRelative("m_Value");

            Rect fieldRect = position;
            fieldRect.height = UnityEditor.EditorGUIUtility.singleLineHeight;
            fieldRect.width = 100;

            UnityEditor.EditorGUI.LabelField(fieldRect, label);

            fieldRect.x += 110;

            UnityEditor.EditorGUI.PropertyField(fieldRect, value, GUIContent.none);

            UnityEditor.EditorGUI.EndProperty();
        }
    }
}
#endif
