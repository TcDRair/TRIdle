using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using UnityEditor;

namespace TRIdle.Editor {
  using Knowledge;
  using Diff = RichTextUtility.Difference;

  public partial class ELayout {
    #region Private Utilities
    private readonly static GUIStyle RichTextArea = new(EditorStyles.textArea) {
      richText = true,
      wordWrap = true
    };

    // TODO various styles
    #endregion

    #region Private Data
    private readonly static Dictionary<int, int> EditorID = new();
    private static int EditorIndex, SelectIndex;
    private static Keyboard KInput => Keyboard.current;
    #endregion


    public static string DynamicTextArea(object owner, string text, params GUILayoutOption[] options) {
      var stream = EKeywordUtility.ParseToDynamic(text);
      return DynamicTextArea(owner, stream, options);
    }
    public static string DynamicTextArea(object owner, DynamicText[] stream, params GUILayoutOption[] options) {
      var plain = string.Concat(stream.Select(x => x.Text));
      var displayText = string.Concat(stream.Select(x => x.DisplayText));
      // All Editing will be done here
      EditorGUILayout.TextArea(displayText, RichTextArea, options);


      return IsFocused(out var editor)
        ? KeywordEditorAction(editor, displayText) // Edit will affect plain text
        : plain;

      bool IsFocused(out TextEditor editor) {
        if ((editor = GetTextEditor()) is not null) {
          // Registeration
          var ID = owner.GetHashCode();
          if (!EditorID.ContainsKey(ID) && editor.text == displayText) EditorID.Add(ID, editor.controlID);

          // Focused
          if (EditorID.TryGetValue(ID, out int CID) && CID == editor.controlID) return true;
        }
        return false;
      }
    }

    private static bool SelectMode;
    private static Vector2 previousMousePosition;
    private static string KeywordEditorAction(TextEditor editor, string before) {
      // 1. Reserved Input Control
      RevertIntellisenseControlInEdit(editor);

      // 2. Revert Invalid Edit
      Diff diff2 = RichTextUtility.FindDifference(before, editor.text);
      Diff diff = RichTextUtility.GetRefinedOutput(diff2, editor.cursorIndex);

      // 3. Removing completed keyword token => Select whole keyword Instead
      string plain = SelectKeywordInsteadOfRemoving(diff, editor).ToString().RemoveTagTokens();

      // 4. Keyword Intellisense
      KeywordIntellisenseLayout(editor, plain);

      // 5. Display text
      editor.text = string.Concat(EKeywordUtility.ParseToDynamic(plain).Select(x => x.DisplayText));

      // 6. Update Properties
      EditorIndex = editor.cursorIndex;

      return plain;

      #region Local Functions
      static void RevertIntellisenseControlInEdit(TextEditor editor) {
        if (SelectMode is false) return;
        
        if (KInput.upArrowKey.isPressed || KInput.downArrowKey.isPressed) {
          editor.cursorIndex = editor.selectIndex = EditorIndex;
        }
        if (KInput.tabKey.HasKeyDown_Editor()) {
          // Remove Tab Key
        }
        
        if (Keyboard.current.escapeKey.HasKeyDown_Editor())
          SelectMode = false;
      }

      static Diff SelectKeywordInsteadOfRemoving(Diff plain, TextEditor editor) {
        if (plain.removed is "]") {
          editor.Insert('[');
          int start = plain.prev.LastIndexOf('[');
          if (start >= 0) {
            plain.prev += ']';
            editor.selectIndex = start;
            editor.cursorIndex = plain.prev.Length;
          }
        }

        if (plain.removed is "[") {
          int end = plain.next.IndexOf(']');
          if (end >= 0) {
            plain.next = '[' + plain.next;
            editor.cursorIndex = plain.ToString().Length - plain.next.Length;
            editor.selectIndex = editor.cursorIndex + end + 2; // +2 for '[' and ']'
          }
        }

        return plain;
      }

      static void KeywordIntellisenseLayout(TextEditor editor, string plain) {
        if (KeywordEdit_Type.TryMatch(plain[..editor.cursorIndex], out var typeMatch)) {
          SelectMode = true;
          var types = Enum.GetNames(typeof(KeywordType));
          var type = typeMatch.Groups["Type"].Value;

          Rect rect = new(GetEditorPosition(editor, typeMatch.Index), default);
          DrawIntellisenseRect(rect, type, types);
        }
        else if (KeywordEdit_Name.TryMatch(plain[..editor.cursorIndex], out var nameMatch)) {
          SelectMode = true;
          var names = Enum.GetNames(typeof(Keyword));
          var name = nameMatch.Groups["Name"].Value;

          Rect rect = new(GetEditorPosition(editor, nameMatch.Index), default);
          DrawIntellisenseRect(rect, name, names);
        }
        else SelectIndex = 0;
      }

      static Vector2 GetEditorPosition(TextEditor editor, int index)
        => editor.style.GetCursorPixelPosition(editor.position, new(editor.text), index);

      static void DrawIntellisenseRect(Rect rect, string token, IEnumerable<string> list) {
        rect.y += EditorGUIUtility.singleLineHeight;
        rect.size = new(120, 160);

        var startsWith = list.Where(x => x.StartsWith(token, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x);
        var contains = list.Where(x => x.Contains(token, StringComparison.OrdinalIgnoreCase)).Except(startsWith).OrderBy(x => x);
        var remain = list.Except(startsWith).Except(contains).OrderBy(x => x);

        // Selection Control
        ApplyIntellisenseControl(token, list);

        // Draw Layouts
        Color background = new(.3f, .3f, .3f), selected = new(.25f, .25f, .25f);
        GUILayout.BeginArea(rect);
        GUILayout.BeginVertical(EStyle.Background(background));

        DrawLayout();

        GUILayout.EndVertical();
        GUILayout.EndArea();

        void DrawLayout() {
          int i = 0;
          foreach (var item in startsWith) {
            if (i == SelectIndex) GUILayout.Label(item, EStyle.Background(selected, "Label"));
            else GUILayout.Label(item);
            i++;
          }
          foreach (var item in contains) {
            if (i == SelectIndex) GUILayout.Label(item, EStyle.Background(selected, "Label"));
            else GUILayout.Label(item);
            i++;
          }
          // foreach (var item in remain) GUILayout.Label(item);
        }
      }

      static void ApplyIntellisenseControl(string token, IEnumerable<string> list) {
        var startsWith = list.Where(x => x.StartsWith(token, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x);
        var contains = list.Where(x => x.Contains(token, StringComparison.OrdinalIgnoreCase)).Except(startsWith).OrderBy(x => x);
        var remain = list.Except(startsWith).Except(contains).OrderBy(x => x);

        int count = startsWith.Count() + contains.Count();
        if (count == 0) return;

        var keyboard = Keyboard.current;
        if (keyboard.upArrowKey.HasKeyDown_Editor())
          SelectIndex = (SelectIndex - 1 + count) % count;
        if (keyboard.downArrowKey.HasKeyDown_Editor())
          SelectIndex = (SelectIndex + 1) % count;
      }

      #endregion
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

  #region Editor Input System
  public static class EditorInputSystemUtility {
    private readonly static Dictionary<KeyControl, bool> pressed = new();
    public static bool HasKeyDown_Editor(this KeyControl control) {
      bool has = pressed.TryGetValue(control, out bool value) && value;
      return (pressed[control] = control.isPressed) && !has;
    }
  }
  #endregion

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