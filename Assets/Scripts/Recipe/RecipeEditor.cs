using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

using TRIdle.Game.Item;

namespace TRIdle.Game.Recipe
{
  [CustomEditor(typeof(RecipeSerialized))]
  public class RecipeEditor : Editor
  {
    float Space => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    GUIStyle Bold => EditorStyles.boldLabel;
    ReorderableList requirements, tools;
    protected void OnEnable() {
      #region Requirements
      requirements = new(serializedObject, serializedObject.FindProperty("requirements"), true, true, true, true) {
        drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Requirements", Bold)
      };
      requirements.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
        var line = rect; line.height = EditorGUIUtility.singleLineHeight;
        var prop = requirements.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(line, prop);
        if (EditorGUI.EndChangeCheck())
          serializedObject.ApplyModifiedProperties();
      };
      requirements.elementHeightCallback = (int index) => {
        var item = requirements.serializedProperty.GetArrayElementAtIndex(index);
        var foldout = item.FindPropertyRelative("_foldout");
        if (foldout.boolValue is false) return Space;
        var use = item.FindPropertyRelative("levelRange.use");
        var tagHeight = EditorGUI.GetPropertyHeight(item.FindPropertyRelative("tags"));
        var attrHeight = EditorGUI.GetPropertyHeight(item.FindPropertyRelative("attributes"));
        return (Space * 6) + tagHeight + attrHeight;
      };
      #endregion

      #region Tools
      tools = new(serializedObject, serializedObject.FindProperty("tools"), true, true, true, true);
      tools.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Tools", Bold);
      tools.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
        var line = rect; line.height = EditorGUIUtility.singleLineHeight;
        var prop = tools.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(line, prop);
        if (EditorGUI.EndChangeCheck())
          serializedObject.ApplyModifiedProperties();
      };
      tools.elementHeightCallback = (int index) => {
        var item = tools.serializedProperty.GetArrayElementAtIndex(index);
        var foldout = item.FindPropertyRelative("_foldout");
        if (foldout.boolValue is false) return Space;
        var use = item.FindPropertyRelative("levelRange.use");
        var tagHeight = EditorGUI.GetPropertyHeight(item.FindPropertyRelative("tags"));
        var attrHeight = EditorGUI.GetPropertyHeight(item.FindPropertyRelative("attributes"));
        return (Space * 6) + tagHeight + attrHeight;
      };
      #endregion
    }

    public override void OnInspectorGUI() {
      var recipe = target as RecipeSerialized;

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Recipe Name", Bold);
      recipe.uName = EditorGUILayout.TextField(recipe.uName);
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Tooltip", Bold);
      recipe.tooltip = EditorGUILayout.TextArea(recipe.tooltip, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Inputs", Bold);
      // base.OnInspectorGUI();
      requirements.DoLayoutList();
      tools.DoLayoutList();
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Result", Bold);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("Result"));

      serializedObject.ApplyModifiedProperties();
    }
  }

  [CustomPropertyDrawer(typeof(RequiredItem))]
  public class RequiredItemDrawer : PropertyDrawer {
    float Space => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      var foldout = property.FindPropertyRelative("_foldout");
      var uName = property.FindPropertyRelative("uName");
      var amount = property.FindPropertyRelative("amount");
      var levelRange = property.FindPropertyRelative("levelRange");
      var cap = levelRange.FindPropertyRelative("cap");
      cap.intValue = 100; // Item Level Cap
      var tags = property.FindPropertyRelative("tags");
      var attributes = property.FindPropertyRelative("attributes");
      var line = position; line.height = EditorGUIUtility.singleLineHeight;

      EditorGUI.BeginChangeCheck();
      // Draw
      Indent(ref line, false);
      string nameLabel = $"{(uName.stringValue == "" ? "(item)" : uName.stringValue)} × {amount.intValue}";
      nameLabel += LevelRangeDrawer.LevelRangeIndicator(levelRange);
      foldout.boolValue = EditorGUI.Foldout(line, foldout.boolValue, nameLabel, true);
      if (foldout.boolValue) {
        Indent(ref line);
        uName.stringValue = EditorGUI.TextField(line, "Name", uName.stringValue);
        line.y += Space;
        amount.intValue = Mathf.Clamp(EditorGUI.IntField(line, "Amount", amount.intValue), 1, 100);
        line.y += Space;
        EditorGUI.PropertyField(line, levelRange);
        line.y += Space * 3;
        EditorGUI.PropertyField(line, tags);
        line.y += EditorGUI.GetPropertyHeight(tags);
        EditorGUI.PropertyField(line, attributes);
      }
      // line.y += Space;

      // Save
      if (EditorGUI.EndChangeCheck())
        property.serializedObject.ApplyModifiedProperties();
    }

    const int IndentWidth = 15;
    void Indent(ref Rect rect, bool newline = true, bool reverse = false) {
      rect.x += reverse ? -IndentWidth : IndentWidth;
      rect.width -= reverse ? -IndentWidth : IndentWidth;
      if (newline) rect.y += Space;
    }
  }

  [CustomPropertyDrawer(typeof(RequiredTag))]
  public class RequiredTagDrawer : PropertyDrawer {
    float Space => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      var foldout = property.FindPropertyRelative("_foldout");
      var tag = property.FindPropertyRelative("data");
      var levelRange = property.FindPropertyRelative("levelRange");
      var use = levelRange.FindPropertyRelative("use");
      var min = levelRange.FindPropertyRelative("min");
      var max = levelRange.FindPropertyRelative("max");
      var cap = levelRange.FindPropertyRelative("cap");
      var line = position; line.height = EditorGUIUtility.singleLineHeight;
      bool tagAssigned = tag.objectReferenceValue != null;
      if (tagAssigned) {
        cap.intValue = ((TagData)tag.boxedValue).info.maxLevel;
      } else {
        // Reset if Tag is unassigned
        cap.intValue = 1;
        use.boxedValue = false;
        min.intValue = 1;
        max.intValue = 1;
      }

      EditorGUI.BeginChangeCheck();
      var tagLabel = (tagAssigned) ? tag.objectReferenceValue.name : "Tag";
      tagLabel += LevelRangeDrawer.LevelRangeIndicator(levelRange);
      foldout.boolValue = EditorGUI.Foldout(line, foldout.boolValue, tagLabel, true);
      if (foldout.boolValue) {
        Indent(ref line);
        EditorGUI.PropertyField(line, tag); // TagData File Selector
        line.y += Space;
        GUI.enabled = tagAssigned;
        EditorGUI.PropertyField(line, levelRange);
      }

      if (EditorGUI.EndChangeCheck())
        property.serializedObject.ApplyModifiedProperties();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
      => property.FindPropertyRelative("_foldout").boolValue ? (Space * 5) : Space;

    const int IndentWidth = 15;
    void Indent(ref Rect rect, bool newline = true, bool reverse = false) {
      rect.x += reverse ? -IndentWidth : IndentWidth;
      rect.width -= reverse ? -IndentWidth : IndentWidth;
      if (newline) rect.y += Space;
    }
  }

  [CustomPropertyDrawer(typeof(LevelRange))]
  public class LevelRangeDrawer : PropertyDrawer
  {
    const float INDENT = 15;
    float Space => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      var use = property.FindPropertyRelative("use");
      var min = property.FindPropertyRelative("min");
      var max = property.FindPropertyRelative("max");
      var cap = property.FindPropertyRelative("cap").intValue; // Defined in Other Editor Script
      var line = position; line.height = EditorGUIUtility.singleLineHeight;

      EditorGUI.BeginChangeCheck();
      use.boolValue = EditorGUI.Toggle(line, "Use Level Range", use.boolValue);
      GUI.enabled = use.boolValue;
      line.x += INDENT;
      line.width -= INDENT;
      line.y += Space;
      min.intValue = Mathf.Clamp(EditorGUI.IntSlider(line, "Min", min.intValue, 1, cap), 1, max.intValue);
      line.y += Space;
      max.intValue = Mathf.Clamp(EditorGUI.IntSlider(line, "Max", max.intValue, 1, cap), min.intValue, cap);
      GUI.enabled = true;

      if (EditorGUI.EndChangeCheck())
        property.serializedObject.ApplyModifiedProperties();
    }

    public static string LevelRangeIndicator(SerializedProperty levelRange) {
      if (levelRange.type != "LevelRange") return " (Invalid Property Type)";
      var use = levelRange.FindPropertyRelative("use").boolValue;
      if (use is false) return "";
      var min = levelRange.FindPropertyRelative("min").intValue;
      var max = levelRange.FindPropertyRelative("max").intValue;
      var cap = levelRange.FindPropertyRelative("cap").intValue;
      bool min1 = min == 1, maxCap = max == cap, minMax = min == max;
      return (min1, maxCap, minMax) switch {
        (_, _, true) => $" [Lv.{min}]",
        (true, true, _) => "",
        (true, false, _) => $" [ ~ Lv.{max}]",
        (false, true, _) => $" [Lv.{min} ~ ]",
        _ => $" [Lv.{min} ~ Lv.{max}]"
      };
    }
  }
}
