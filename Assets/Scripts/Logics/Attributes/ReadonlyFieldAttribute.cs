using System;

using UnityEngine;
using UnityEditor;

namespace TRIdle.Logics.Attributes
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
  public class ReadonlyFieldAttribute : PropertyAttribute
  {
    public ReadonlyFieldAttribute() { }

    [CustomPropertyDrawer(typeof(ReadonlyFieldAttribute))]
    public class ReadonlyFieldAttributeEditor : PropertyDrawer
    {
      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        bool e;
        (e, GUI.enabled) = (GUI.enabled, false);
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = e;
      }
    }
  }
}