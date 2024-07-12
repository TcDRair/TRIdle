using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TRIdle.Editor {
  public class RichTextUtility {
    public struct Difference {
      public int start, end;
      public string prev, added, removed, next;
      public string restored;

      public readonly bool IsAdded => added?.Length > 0;
      public readonly bool IsRemoved => removed?.Length > 0;
      public readonly bool IsRemovedMany => removed?.Length > 1;

      public override readonly string ToString() => $"{prev}{added}{restored}{next}";
      public readonly string ToPlainString() => GetPlainText(ToString());
      public readonly Difference ToPlain() {
        Difference plain = new() {
          prev = GetPlainText(prev, false),
          added = GetPlainText(added, false),
          removed = GetPlainText(removed, false),
          next = GetPlainText(next, false),
          restored = GetPlainText(restored, false)
        };
        plain.start = plain.prev.Length;
        plain.end = plain.ToString().Length - plain.next.Length;
        return plain;
      }
      public readonly string Explain(bool rich = true)
        => rich ? Exp() : Exp().Replace("<", "{").Replace(">", "}");
      readonly string Exp()
        => added.Length == 0 && removed.Length == 0
          ? "Difference: Not changed"
          : $"Difference: {start} {ToString().Length - end}\n" +
            $"Prev: {prev}\n" +
            $"Added: {added}\n" +
            $"Restored: {restored}\n" +
            $"Next: {next}\n" +
            $"Removed: {removed}\n" +
            $"Plain: {ToPlainString()}\n" +
            $"Valid?: {IsValidRichText(ToString(), out _)}";
    }

    /// <summary>
    /// Correct the difference between before and after text.
    /// Only consider the difference occurs once and continuously.
    /// </summary>
    /// <param name="before">text before editing</param>
    /// <param name="after">text after editing</param>
    /// <param name="cursorIndex">cursor index after editing</param>
    /// <example>
    /// These samples shows <see cref="GetRefinedOutput(string, string, int)"/> usage.
    /// <code>
    /// RichTextUtility.Correct("12345", "145", 2) // => "145"
    /// RichTextUtility.Correct("1<b>2</b>345", "1b>2</b>345", 1) // => "1<b></b>345"
    /// RichTextUtility.Correct("1<b>23</b>45", "13</b>45", 1) // => "1<b>3</b>45"
    /// </code>
    /// </example>
    public static Difference GetRefinedOutput(string before, string after, int cursorIndex) {
      if (IsValidRichText(before, out var plain) is false) throw new ArgumentException("Invalid rich text", nameof(before));

      var diff = FindDiff(before, after);

      // Addition:  Simply remove all tag brackets(+ complete tags) added
      if (diff.IsAdded) diff.added = RegexPattern.DetectTagAddition.Replace(diff.added, "");
      // Selection Removal: Restore the removed tag brackets
      if (diff.IsRemovedMany)
        diff.restored = string.Concat(
          RegexPattern.DetectTagRemoval.Matches(diff.removed)
            .Select(m => m.Groups[RegexName.AnyTag].Value)
        );
      // Single Removal: Propagate the removal ouside tag brackets
      else if (diff.IsRemoved) PropagateRemoval();

      return diff;

      void PropagateRemoval() {
        // If the removed char is not a tag bracket, the action is valid
        if (diff.removed is not ("<" or ">")) return;

        // Backspace: Remove the last valid letter in the pre text
        var indexes = RichTextIndex(before);
        bool isBackspace = cursorIndex < indexes.Length && indexes[cursorIndex] < diff.start;
        if (isBackspace) {
          diff.prev = $"{diff.prev}{diff.removed}"; // Add the last char
          var match = RegexPattern.TagSearchFromEnd.Match(diff.prev);
          if (match.Success && match.Index > 0) {
            // match.Index - 1 index will be deleted
            diff.removed = $"{diff.prev[match.Index - 1]}";
            diff.next = diff.prev[match.Index..] + diff.next;
            diff.prev = diff.prev[..match.Index];
          }
          else diff.prev = diff.prev[..^1]; // Remove the last char
        }
        // Delete: Remove the first valid letter in the post text
        else {
          diff.next = $"{diff.removed}{diff.next}"; // Add the first char
          var match = RegexPattern.TagSearchFromStart.Match(diff.next);
          int revIdx = diff.next.Length - match.Index - match.Length;
          if (match.Success && revIdx > 1) {
            diff.removed = $"{diff.next[^revIdx]}";
            diff.prev += diff.next[..^revIdx];
            diff.next = diff.next[^(revIdx - 1)..];
          }
          else diff.next = diff.next[1..]; // Remove the first char
        }
      }

      static string Plain(string rich) => RegexPattern.AnyTag.Replace(rich, "");

      static Difference FindDiff(string before, string after) {
        if (Plain(after).Length == 0) return new() { removed = before };

        int start, end;
        int min = Math.Min(before.Length, after.Length), max = Math.Max(before.Length, after.Length);
        // Find the first difference
        for (start = 0; start < min; start++)
          if (before[start] != after[start])
            break;
        // Find the last difference
        for (end = 1; end < max - start; end++)
          if (before[^end] != after[^end])
            break;
        end--;

        return new() {
          start = start,
          end = end,

          prev = (start > 0) ? after[..start] : string.Empty,
          removed = (start + end < before.Length) ? before[start..^end] : string.Empty,
          added = (start + end < after.Length) ? after[start..^end] : string.Empty,
          next = (end > 0) ? after[^end..] : string.Empty
        };
      }
    }

    #region Regex Utility
    private class RegexName {
      public const string
        TagName = "Name",
        TagValue = "Value",
        TagBody = "Body",
        TagTail = "Tail",
        TagHead = "Head",
        FullTag = "Full",
        AnyTag = "Any";
    }
    private class RegexPattern {
      private const string Tokens = @"<>=/", All = @"\s\S";
      private static readonly string TagTail_Dependent = $@"(?<{RegexName.TagTail}><\/\k<{RegexName.TagName}>>)";
      private const RegexOptions
        Default = RegexOptions.Compiled,
        Reverse = Default | RegexOptions.RightToLeft;

      public static readonly Regex
        TagName = new($@"(?<{RegexName.TagName}>[^{Tokens}]+?)", Default),
        TagValue = new($@"(?<{RegexName.TagValue}>[^{Tokens}]+?)", Default),
        TagHead = new($@"(?<{RegexName.TagHead}><{TagName}(?:={TagValue})?>)", Default),
        TagBody = new($@"(?<{RegexName.TagBody}>[{All}]*?)", Default),
        TagTail = new($@"(?<{RegexName.TagTail}><\/{TagName}>)", Default),
        FullTag = new($@"(?<{RegexName.FullTag}>{TagHead}{TagBody}{TagTail_Dependent})", Default),
        AnyTag = new($@"(?<{RegexName.AnyTag}>{TagHead}|{TagTail})", Default),
        TagToken = new($@"([{Tokens}])", Default);

      public static readonly Regex
        TagSearchFromStart = new($@"(^{AnyTag}+)", Default),
        TagSearchFromEnd = new($@"({AnyTag}+$)", Reverse),
        DetectTagRemoval = new($@"{AnyTag}|(^[^{Tokens}]*?>)|(<[^{Tokens}]*?$)", Default),
        DetectTagAddition = new($@"{AnyTag}|{TagToken}", Default);
    }

    /// <summary>
    /// <para>Check if the text is a valid rich text.</para>
    /// <para>All tags must be closed properly, and there should be no other tokens.</para>
    /// </summary>
    static bool IsValidRichText(string text, out string plainText) {
      plainText = GetPlainText(text);
      return RegexPattern.AnyTag.IsMatch(plainText) is false;
    }
    public static string GetPlainText(string richText, bool removeValidTagsOnly = true) {
      if (richText is null) return string.Empty;
      string result = richText;

      if (removeValidTagsOnly)
        // Remove all complete tags
        while (RegexPattern.FullTag.TryReplace(result, out result, m => m.Groups[RegexName.TagBody].Value));
      else
        // Remove all tags
        while (RegexPattern.AnyTag.TryReplace(result, out result, ""));

      return result;
    }
    static int[] RichTextIndex(string rich) {
      if (IsValidRichText(rich, out var plain) is false) throw new ArgumentException("Invalid rich text", nameof(rich));
      int[] indexes = new int[plain.Length + 1];
      for (int p = 0, r = 0; p < plain.Length; p++, r++) {
        if (RegexPattern.TagSearchFromStart.Match(rich[r..]) is Match m && m.Success && m.Index == 0)
          r += m.Length;
        indexes[p] = r;
      }
      indexes[^1] = rich.Length;
      return indexes;
    }
    #endregion
  }

  public static class Ext {
    public static string ToPlainString(this string richText) => RichTextUtility.GetPlainText(richText);
    public static string RemoveTagTokens(this string text) => text.Replace("<", "").Replace(">", "");
    public static bool TryMatch(this Regex regex, string input, out Match match)
      => (match = regex.Match(input)).Success;
    
    public static bool TryReplace(this Regex regex, string input, out string result, string replacement) {
      if (regex.IsMatch(input)) {
        result = regex.Replace(input, replacement);
        return true;
      }
      result = input;
      return false;
    }
    public static bool TryReplace(this Regex regex, string input, out string result, MatchEvaluator evaluator) {
      if (regex.TryMatch(input, out var match)) {
        result = regex.Replace(input, evaluator);
        return true;
      }
      result = input;
      return false;
    }
  }
}