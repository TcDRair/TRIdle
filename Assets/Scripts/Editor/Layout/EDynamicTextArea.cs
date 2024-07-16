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
    
    private struct EditorState {
      public string text;
      public int cursorIndex, selectIndex;
      public void Save(TextEditor editor) {
        text = editor.text;
        cursorIndex = editor.cursorIndex;
        selectIndex = editor.selectIndex;
      }
      public readonly void Load(TextEditor editor) {
        editor.text = text;
        editor.cursorIndex = cursorIndex;
        editor.selectIndex = selectIndex;
      }
    }
    #endregion

    #region Private Data
    private readonly static Dictionary<int, int> EditorID = new();
    private static int EditorIndex, SelectIndex;
    private static Keyboard KInput => Keyboard.current;
    #endregion

    #region Public Layout Method
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
    #endregion

    #region Temporary Variables
    private static bool SelectMode, SelectEscape;
    private static EditorState State;
    #endregion
    private static string KeywordEditorAction(TextEditor editor, string before) {
      // 1. Reserved Input Control
      RevertIntellisenseControlInEdit(editor, before);

      // 2. Revert Invalid Edit
      Diff diff = RichTextUtility.GetRefinedOutput(before, editor.text, editor.cursorIndex);

      // 3. Select Keyword Instead of Removing while backspacing/deleting brackets('[', ']')
      SelectKeywordInsteadOfRemoving(editor, diff);

      // 4. Keyword Intellisense (Layout, auto-complete)
      KeywordIntellisenseLayout(editor, diff);

      // 5. Display text
      PrintDynamicText(editor, diff);

      // 6. Update Properties
      EditorIndex = editor.cursorIndex;
      State.Save(editor);

      return diff.Result;

      #region Local Functions
      static void RevertIntellisenseControlInEdit(TextEditor editor, string before) {
        RevertTabControl(editor);
        if (SelectMode || SelectEscape) return;

        if (KInput.upArrowKey.isPressed || KInput.downArrowKey.isPressed)
          Debug.Log(editor.cursorIndex = editor.selectIndex = EditorIndex);

        // Remove Tab ('\t' char input will be done after one frame, so manually remove it)
        editor.text = editor.text.Replace("\t", "");

        SelectEscape = Keyboard.current.escapeKey.HasKeyDown_Editor();
      }

      static void SelectKeywordInsteadOfRemoving(TextEditor editor, Diff diff) {
        if (diff.removed is "]") {
          editor.Insert('[');
          int start = diff.prev.LastIndexOf('[');
          if (start >= 0) {
            diff.prev += ']';
            editor.selectIndex = start;
            editor.cursorIndex = diff.prev.Length;
          }
        }

        if (diff.removed is "[") {
          int end = diff.next.IndexOf(']');
          if (end >= 0) {
            diff.next = '[' + diff.next;
            editor.cursorIndex = diff.ToString().Length - diff.next.Length;
            editor.selectIndex = editor.cursorIndex + end + 2; // +2 for '[' and ']'
          }
        }
      }

      static void KeywordIntellisenseLayout(TextEditor editor, Diff diff) {
        string plain = diff.Result;
        var index = Math.Min(editor.cursorIndex, plain.Length);
        if (KeywordEdit_Type.TryMatch(plain[..index], out var typeMatch)) {
          SelectMode = true;
          var types = Enum.GetNames(typeof(KeywordType));
          var type = typeMatch.Groups["Type"].Value;

          Rect rect = new(GetEditorPosition(editor, typeMatch.Index), default);
          DrawIntellisenseRect(editor, diff, rect, type, types, ":");
        }
        else if (KeywordEdit_Name.TryMatch(plain[..index], out var nameMatch)) {
          SelectMode = true;
          var names = Enum.GetNames(typeof(Keyword));
          var name = nameMatch.Groups["Name"].Value;

          Rect rect = new(GetEditorPosition(editor, nameMatch.Index), default);
          DrawIntellisenseRect(editor, diff, rect, name, names, "]");
        }
        else {
          SelectMode = SelectEscape = false;
          SelectIndex = 0;
        }
      }

      static void DrawIntellisenseRect(TextEditor editor, Diff diff, Rect rect, string token, IEnumerable<string> list, string append = "") {
        rect.y += EditorGUIUtility.singleLineHeight;
        rect.size = new(120, 160);

        var startsWith = list.Where(x => x.StartsWith(token, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x);
        var contains = list.Where(x => x.Contains(token, StringComparison.OrdinalIgnoreCase)).Except(startsWith).OrderBy(x => x);
        // var remain = list.Except(startsWith).Except(contains).OrderBy(x => x);
        var orderedList = startsWith.Concat(contains);

        // Selection Control
        ApplyIntellisenseControl(editor, diff, token, orderedList, append);

        // Draw Layouts
        DoList(rect, orderedList, SelectIndex);

        static void DoList(Rect rect, IEnumerable<string> orderedList, int SelectIndex) {
          Color background = new(.3f, .3f, .3f), selected = new(.25f, .25f, .25f);
          GUILayout.BeginArea(rect);
          GUILayout.BeginVertical(EStyle.Background(background));

          int i = 0;
          foreach (var item in orderedList) {
            if (i == SelectIndex) GUILayout.Label(item, EStyle.Background(selected, "Label"));
            else GUILayout.Label(item);
            i++;
          }

          GUILayout.EndVertical();
          GUILayout.EndArea();
        }
      }

      static void ApplyIntellisenseControl(TextEditor editor, Diff diff, string token, IEnumerable<string> orderedList, string append = "") {
        int count = orderedList.Count();
        if (count == 0) return;

        var keyboard = Keyboard.current;
        if (keyboard.upArrowKey.HasKeyDown_Editor())
          SelectIndex = (SelectIndex - 1 + count) % count;
        if (keyboard.downArrowKey.HasKeyDown_Editor())
          SelectIndex = (SelectIndex + 1) % count;
        if (keyboard.tabKey.HasKeyDown_Editor()) {
          var selected = orderedList.ElementAt(SelectIndex);
          diff.prev = diff.prev[..^token.Length];
          diff.added += selected + append;
          editor.text = diff.Result;
          editor.cursorIndex += selected.Length + append.Length - token.Length;
          editor.selectIndex = editor.cursorIndex;
        }
      }

      static void PrintDynamicText(TextEditor editor, Diff diff)
        => editor.text = string.Concat(EKeywordUtility.ParseToDynamic(diff.Result).Select(t => t.DisplayText));

      static Vector2 GetEditorPosition(TextEditor editor, int index)
        => editor.style.GetCursorPixelPosition(editor.position, new(editor.text), index);

      static void RevertTabControl(TextEditor editor) {
        if (editor.text.Contains('\t')) State.Load(editor);
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