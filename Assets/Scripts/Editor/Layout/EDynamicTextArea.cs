using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

namespace TRIdle.Editor {
  using Knowledge;

  public partial class ELayout {
    private readonly static Regex RichTextRegex = new(@"<.*?>");
    private readonly static Dictionary<int, int> EditorID = new();
    private readonly static GUIStyle RichTextArea = new(EditorStyles.textArea) {
      richText = true,
      wordWrap = true
    };
    
    public static string DynamicTextArea(object owner, string text, params GUILayoutOption[] options) {
      var stream = EKeywordUtility.ParseToDynamic(text);
      return DynamicTextArea(owner, stream, options);
    }
    public static string DynamicTextArea(object owner, DynamicText[] stream, params GUILayoutOption[] options) {
      var plain = string.Concat(stream.Select(x => x.Text));
      var display = string.Concat(stream.Select(x => x.DisplayText));

      string before = display;

      EditorGUILayout.TextArea(display, RichTextArea, options);

      if (GetTextEditor() is TextEditor editor) {
        var ID = owner.GetHashCode();
        // Registeration
        if (!EditorID.ContainsKey(ID) && editor.text == display)
          // Unless the plain text is modified, corresponding ID will be called
          EditorID.Add(ID, editor.controlID);
        // On Focus
        if (EditorID.TryGetValue(ID, out int CID) && CID == editor.controlID) {
          // Custom Action
          if (GetElementAt(stream, editor.cursorIndex) is DynamicText element)
            element.OnFocused();
          var edit = editor.text;
          // TODO : 다음 조건 만족
          // 1. [] token as whole (removed entirely)
          // 2. <> token as private (prevent adding or removing) (completed)
          // 3. [ token as progress (adding invokes proper closing)
          var after = RichTextUtility.Correct(before, edit, editor.cursorIndex);
          plain = RichTextRegex.Replace(after, string.Empty);
          editor.text = string.Concat(EKeywordUtility.ParseToDynamic(plain).Select(x => x.DisplayText));
        }
      }

      return plain;
    }

    public static DynamicText GetElementAt(DynamicText[] stream, int caret) {
      int position = 0;
      foreach (var text in stream) {
        position += text.Text.Length;
        if (caret <= position) return text;
      }
      return null;
    }
    private static TextEditor GetTextEditor() {
      var editorType = typeof(EditorGUI);
      var activeEditorField = editorType.GetField("activeEditor", BindingFlags.Static | BindingFlags.NonPublic);
      var activeEditor = activeEditorField.GetValue(null) as TextEditor;
      return activeEditor;
    }
  }
  public partial class EKeywordUtility {
    public static readonly Regex KeywordRegex = new(@"\[(?<Type>[^\[\]:]+):(?<Name>[^\[\]:]+)\]"); // [Type:Key]
    public static DynamicText[] ParseToDynamic(string text) {
      var keywords = KeywordRegex
        .Matches(text)
        .Select(m => new KeywordText(m.Groups["Type"].Value, m.Groups["Name"].Value))
        .ToArray();
      var remains = KeywordRegex
        .Replace(text, "$#%")
        .Split("$#%") // This will split string without capture group
        .Select(s => new PlainText(s));

      var results = new List<DynamicText>(remains);

      for (int i = keywords.Length; i > 0; i--) results.Insert(i, keywords[i - 1]);
      return results.ToArray();
    }
  }

  public abstract class DynamicText {
    public string Text { get; protected set; }
    public string DisplayText { get; protected set; }
    public virtual void Layout(Rect rect) { }
    public virtual void OnFocused() { }
  }

  public class PlainText : DynamicText {
    public PlainText(string text) => Text = DisplayText = text;
    public override void OnFocused() {

    }
  }

  public class KeywordText : DynamicText {
    public readonly KeywordBase Info; // Set as variable for 'out'
    public override void Layout(Rect rect) {
      //TODO Draw Round Rect keyword (like notion)
    }
    public override void OnFocused() {
      // TODO : Overlay
    }

    private string TColor(bool match) => match ? "green" : "red";
    public KeywordText(string type, string text) {
      bool keyMatch = Enum.TryParse(text, out Keyword keyword) && keyword.TryGetKeywordInfo(out Info);
      bool typeMatch = keyMatch && Info.Type.ToString() == type;

      Text = $"[{type}:{text}]";
      DisplayText = $"<b>[<color={TColor(typeMatch)}>{type}</color>:<color={TColor(keyMatch)}>{text}</color>]</b>";
    }
  }
}