using System.Linq;

using UnityEngine;
using UnityEditor;

namespace TRIdle.Editor
{
  public class EStyle
  {
    static GUIStyle _label, _button, _boldLabel, _richText, _boldCenterLabel, _headerTab, _background;
    public static GUIStyle Label => _label ??= GUI.skin.label;
    public static GUIStyle Button => _button ??= EditorStyles.miniButton;
    public static GUIStyle BoldLabel => _boldLabel ??= new(Label) { fontStyle = FontStyle.Bold };
    public static GUIStyle BoldCenterLabel => _boldCenterLabel ??= new(BoldLabel) { alignment = TextAnchor.MiddleCenter };

    public static GUIStyle RichText => _richText ??= new(EditorStyles.wordWrappedLabel) { richText = true };

    public static GUIStyle HeaderTabOn => new("RL Empty Header") {
      alignment = TextAnchor.LowerCenter,
      contentOffset = new(0, -3),
      fixedHeight = 24, // EditorGUI.kLargeButtonHeight,
      stretchWidth = true,

      font = Label.font,
      fontSize = Label.fontSize,
      fontStyle = FontStyle.Bold
    };
    public static GUIStyle HeaderTabOff => new("RL Header") {
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
  }
}