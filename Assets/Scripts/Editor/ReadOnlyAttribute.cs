using UnityEngine;
using UnityEditor;

namespace TRIdle {
  /// <summary>
  /// Attribute to make a field read-only in the inspector.
  /// Reveals the field but does not allow editing.
  /// </summary>
  [System.AttributeUsage(System.AttributeTargets.Field)]
  public class ReadOnlyAttribute : PropertyAttribute {
  }

  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
  public class ReadOnlyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      GUI.enabled = false;
      EditorGUI.PropertyField(position, property, label, true);
      GUI.enabled = true;
    }
  }
}