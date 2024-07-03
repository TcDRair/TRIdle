using System.Linq;

using UnityEngine;
using UnityEditor;

namespace TRIdle.Editor
{
  public class EStyle
  {
    #region Custom GUIStyles
    static GUIStyle _label, _button, _boldLabel, _richText, _boldCenterLabel, _headerTabOn, _headerTabOff;
    /// <summary>Same as <see cref="EditorStyles.label"/></summary>
    public static GUIStyle Label => _label ??= EditorStyles.label;
    /// <summary>Same as <see cref="EditorStyles.boldLabel"/></summary>
    public static GUIStyle BoldLabel => _boldLabel ??= EditorStyles.boldLabel;
    /// <summary>Centered <see cref="EditorStyles.boldLabel"/></summary>
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
    
    public static GUIStyle Background(int depth = 0) => new() {
      border = new(1, 1, 1, 1),
      padding = new(0, 0, 0, 0),
      normal = new() { background = (depth % 2 > 0 ? .2f : .25f).ToColor().ToTexture2D() },
    };
  
    #endregion

    #region Custom Layouts
    public static void FlexibleHeight() => GUILayout.Label(GUIContent.none, GUILayout.ExpandHeight(true));
    public static void FlexibleWidth() => GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true));
    #endregion
  }
}