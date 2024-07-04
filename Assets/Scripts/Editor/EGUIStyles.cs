using System.Linq;

using UnityEngine;
using UnityEditor;

namespace TRIdle.Editor {
  public class EStyle {
    public static GUIStyle Label => _label ??= EditorStyles.label;
    public static GUIStyle BoldLabel => _boldLabel ??= EditorStyles.boldLabel;
    public static GUIStyle BoldCenterLabel => _boldCenterLabel ??= new(BoldLabel) { alignment = TextAnchor.MiddleCenter };
    public static GUIStyle RichText => _richText ??= new(EditorStyles.wordWrappedLabel) { richText = true };
    public static GUIStyle Button => _button ??= EditorStyles.miniButton;

    public static GUIStyle HeaderTabOn => _headerTabOn ??= new("RL Empty Header") {
      alignment = TextAnchor.LowerCenter,
      contentOffset = new(0, -3),
      fixedHeight = 24, // EditorGUI.kLargeButtonHeight,
      stretchWidth = true,

      font = Label.font,
      fontSize = Label.fontSize,
      fontStyle = FontStyle.Bold
    };
    public static GUIStyle HeaderTabOff => _headerTabOff ??= new("RL Header") {
      alignment = TextAnchor.LowerCenter,
      contentOffset = new(0, -3),
      fixedHeight = 24, // EditorGUI.kLargeButtonHeight,
      stretchWidth = true,

      font = Label.font,
      fontSize = Label.fontSize,
      fontStyle = FontStyle.Bold
    };
    public static GUIStyle KeyStyle => _keyStyle ??= new("RL Element") {
      alignment = TextAnchor.MiddleCenter,
      fontStyle = FontStyle.Bold,
      fontSize = 16,
      fixedHeight = 32,
    };

    public static GUIStyle Background(int depth = 0) => new() {
      border = new(1, 1, 1, 1),
      padding = new(0, 0, 0, 0),
      normal = new() { background = (depth % 2 > 0 ? .2f : .25f).ToColor().ToTexture2D() },
    };


    #region Caching
    static GUIStyle _label, _button, _boldLabel, _richText, _boldCenterLabel, _headerTabOn, _headerTabOff, _keyStyle;
    #endregion
  }

  public class ELayout {
    private static int indent = 0;
    public static void BeginIndent(bool horizontalPadding = true, bool verticalPadding = true) {
      var style = EStyle.Background(++indent);
      style.padding = (horizontalPadding, verticalPadding) switch {
        (true, true) => new(8, 8, 8, 8),
        (true, false) => new(8, 8, 0, 0),
        (false, true) => new(0, 0, 8, 8),
        _ => new(0, 0, 0, 0)
      };
      GUILayout.BeginVertical(style);
    }
    public static void EndIndent() {
      GUILayout.EndVertical();
      indent--;
    }

    public static void FlexibleHeight() => GUILayout.Label(GUIContent.none, GUILayout.ExpandHeight(true));
    public static void FlexibleWidth() => GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true));

    public static T Popup<T>(string label, T selected, T[] values) {
      var index = System.Array.IndexOf(values, selected);
      var newIndex = EditorGUILayout.Popup(label, index, values.Select(v => v.ToString()).ToArray());
      return values[newIndex];
    }
  }


}