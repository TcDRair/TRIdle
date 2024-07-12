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
      var displayText = string.Concat(stream.Select(x => x.DisplayText));

      EditorGUILayout.TextArea(displayText, RichTextArea, options);

      if (GetTextEditor() is TextEditor editor) {
        var ID = owner.GetHashCode();

        // Registeration
        if (!EditorID.ContainsKey(ID) && editor.text == displayText)
          EditorID.Add(ID, editor.controlID);

        // Edit
        if (EditorID.TryGetValue(ID, out int CID) && CID == editor.controlID)
          plain = KeywordEditorAction(editor, displayText); // Edit will affect plain text
      }

      return plain;
    }

    private static string KeywordEditorAction(TextEditor editor, string displayText) {
      var edit = editor.text;

      // 1. <> token as fixed (prevent adding or removing) (completed)
      var diff = RichTextUtility.GetRefinedOutput(displayText, edit, editor.cursorIndex);

      var plainDiff = diff.ToPlain();
      // Debug.Log(plainDiff.Explain());

      // 2. [a:b] token as whole (backspace/delete invoke total selection)
      SelectKeywordInsteadOfRemoving();

      // 3. [ token as progress (adding invokes proper closing)
      KeywordIntellisense();

      // Result
      var plain = plainDiff.ToString().RemoveTagTokens();
      editor.text = string.Concat(EKeywordUtility.ParseToDynamic(plain).Select(x => x.DisplayText));

      return plain;

      void SelectKeywordInsteadOfRemoving() {
        if (plainDiff.removed is "]") {
          int start = plainDiff.prev.LastIndexOf('[');
          if (start >= 0) {
            plainDiff.prev += ']';
            editor.selectIndex = start;
            editor.cursorIndex = plainDiff.prev.Length;
          }
        }
        if (plainDiff.removed is "[") {
          int end = plainDiff.next.IndexOf(']');
          if (end >= 0) {
            plainDiff.next = '[' + plainDiff.next;
            editor.cursorIndex = plainDiff.ToString().Length - plainDiff.next.Length;
            editor.selectIndex = editor.cursorIndex + end + 2; // +2 for '[' and ']'
          }
        }
      }

      void KeywordIntellisense() {
        // TODO : Overlay Window + Caret Position

        // TODO : Enum / StartsWith / Contains => Overlay list + AutoComplete(Tab) + ArrowKey to select
        if (KeywordEdit_Type.TryMatch(plainDiff.prev, out var typeMatch)) {
          var types = Enum.GetNames(typeof(KeywordType));
          var type = typeMatch.Groups["Type"].Value;

          Rect rect = new(GetEditorPosition(typeMatch.Index), default);
          DrawIntellisenseRect(rect, type, types);

          // Find All keyword types starts with or contains above
          // If found, list them
          // If not, list all
        }
        else if (KeywordEdit_Name.TryMatch(plainDiff.prev, out var nameMatch)) {
          // if (nameMatch.Groups["Type"].Success)
          // => Find All keyword of that type and do below
          // nameMatch.Groups["Name"].Value
          // Find All keyword starts with or contains above
          // If found, list them
          // If not, list all
        }
      }

      void DrawIntellisenseRect(Rect rect, string token, IEnumerable<string> list) {
        rect.y += EditorGUIUtility.singleLineHeight;
        rect.size = new(120, 160);

        var startsWith = list.Where(x => x.StartsWith(token, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x);
        var contains = list.Where(x => x.Contains(token, StringComparison.OrdinalIgnoreCase)).Except(startsWith).OrderBy(x => x);
        var remain = list.Except(startsWith).Except(contains).OrderBy(x => x);

        Color background = new(.3f, .3f, .3f);
        GUILayout.BeginArea(rect);
        // TODO : Area Background color
        GUILayout.BeginVertical(EStyle.Background(background));
        {
          foreach (var item in startsWith) GUILayout.Label(item);
          foreach (var item in contains) GUILayout.Label(item);
          // foreach (var item in remain) GUILayout.Label(item);
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
      }
    
      Vector2 GetEditorPosition(int index)
        => editor.style.GetCursorPixelPosition(editor.position, new(editor.text), index);
    }

    #region Text Editor Utility
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
    private readonly static Regex KeywordEdit_Type = new(@"\[(?<Type>[^\[\]:\s\n]*)$");
    private readonly static Regex KeywordEdit_Name = new(@"\[(?<Type>[^\[\]:\s\n]+):(?<Name>[^\[\]:\s\n]*)$");
    #endregion
  }

  #region DynamicText Definitions
  public partial class EKeywordUtility {
    public static readonly Regex
      Keyword = new(@"\[(?<Type>[^\[\]:]+):(?<Name>[^\[\]:]+)\]"), // [Type:Key]
      KeySearchFromEnd = new($@"{Keyword}$");
    public static DynamicText[] ParseToDynamic(string text) {
      var keywords = Keyword
        .Matches(text)
        .Select(m => new KeywordText(m.Groups["Type"].Value, m.Groups["Name"].Value))
        .ToArray();
      var remains = Keyword
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
  #endregion
}