using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TRIdle.Editor {
  public class RichTextUtility {
    #region private utilities
    static readonly Regex
      WholeTag = new(@"<[^\n]+?>", RegexOptions.Multiline),
      TagSearchFromStart = new(@"(^(?:<[^<>]+?>)+)", RegexOptions.Multiline),
      TagSearchFromEnd = new(@"((?:<[^<>]+?>)+$)", RegexOptions.Multiline | RegexOptions.RightToLeft),
      DetectTagRemoval = new(@"(<[^\n]+?>)|(^[^\n]*?>)|(<[^\n]*?$)", RegexOptions.Multiline),
      DetectTagAddition = new(@"(<[^\n]+?>)|([<>])", RegexOptions.Multiline);
    struct Difference {
      public bool added, removed, removedMany;
      public int start, end;
      public string pre, before, after, post;
    }
    #endregion

    /// <summary>
    /// Correct the difference between before and after text.
    /// Only consider the difference occurs once and continuously.
    /// </summary>
    /// <param name="before">text before editing</param>
    /// <param name="after">text after editing</param>
    /// <param name="cursorIndex">cursor index after editing</param>
    /// <example>
    /// These samples shows <see cref="Correct(string, string, int)"/> usage.
    /// <code>
    /// RichTextUtility.Correct("12345", "145", 2) // => "145"
    /// RichTextUtility.Correct("1<b>2</b>345", "1b>2</b>345", 1) // => "1<b></b>345"
    /// RichTextUtility.Correct("1<b>23</b>45", "13</b>45", 1) // => "1<b>3</b>45"
    /// </code>
    /// </example>
    public static string Correct(string before, string after, int cursorIndex) {
      var diff = FindDiff(before, after);
      var plain = WholeTag.Replace(before, "");
      string restore = string.Empty;

      // Addition:  Simply remove all tag brackets(+ complete tags) added
      if (diff.added)
        diff.after = DetectTagAddition.Replace(diff.after, "");
      // Selection Removal: Restore the removed tag brackets
      if (diff.removedMany)
        restore = string.Concat(DetectTagRemoval.Matches(diff.before).Skip(1).Select(x => x.Value));
      // Single Removal: Propagate the removal ouside tag brackets
      else if (diff.removed)
        PropagateRemoval();

      // if (diff.added || diff.removed || diff.removedMany) Debug.Log($"Before : {before}\nAfter : {after}\nDifference: {diff.start}~^{diff.end} / +: {diff.added} -: {diff.removed} --: {diff.removedMany}\nEdit : {diff.pre} / {diff.after} / {restore} / {diff.post}"/*.Replace("<", "<.")*/);
      return $"{diff.pre}{diff.after}{restore}{diff.post}";

      void PropagateRemoval() {
        // If the removed char is not a tag bracket, the action is valid
        if (diff.before is not ("<" or ">")) return;

        // Backspace: Remove the last valid letter in the pre text
        int richIndex = GetRichCursorIndex(before, cursorIndex);
        if (richIndex < diff.start) {
          var pre = $"{diff.pre}{diff.before}";
          var match = TagSearchFromEnd.Match(diff.pre);
          if (match.Success && match.Index > 0)
            diff.pre = pre.Remove(match.Index - 1, 1);
          else
            diff.pre = pre[..^1];
        }
        // Delete: Remove the first valid letter in the post text
        else {
          var post = $"{diff.before}{diff.post}";
          var match = TagSearchFromStart.Match(post);
          if (match.Success && match.Index + match.Length < diff.post.Length)
            diff.post = post.Remove(match.Index + match.Length, 1);
          else
            diff.post = post[1..];
        }
      }

      static Difference FindDiff(string before, string after) {
        if (Plain(after).Length == 0) return new() {
          start = 0,
          end = 0,

          added = false,
          removed = before.Length > 0,
          removedMany = before.Length > 1,

          pre = string.Empty,
          post = string.Empty,
          before = before,
          after = string.Empty
        };

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

        bool added = start + end < after.Length;
        bool removed = start + end < before.Length;
        bool removedMany = start + end < before.Length - 1; // 2+ characters removed

        return new() {
          start = start,
          end = end,

          added = added,
          removed = removed,
          removedMany = removedMany,

          pre = (start > 0) ? after[..start] : string.Empty,
          before = (start + end < before.Length) ? before[start..^end] : string.Empty,
          after = (start + end < after.Length) ? after[start..^end] : string.Empty,
          post = (end > 0) ? after[^end..] : string.Empty
        };
      }
    }

    public static int GetRichCursorIndex(string rich, int cursorIndex) {
      var plain = WholeTag.Replace(rich, "");
      if (cursorIndex == 0) return 0;
      if (cursorIndex == plain.Length) return rich.Length;
      foreach (var idx in GetPlainIndexer())
        if (idx.plain == cursorIndex)
          return idx.rich;
      return rich.Length - 1;

      IEnumerable<(int plain, int rich)> GetPlainIndexer() {
        int pI = 0, rI;
        for (rI = 0; rI < rich.Length; rI++) {
          yield return (pI, rI);
          if (WholeTag.Match(rich[rI..]) is Match m && m.Success && m.Index == 0)
            rI += m.Length;
          pI++;
        }
      }
    }
  
    public static string Plain(string rich) => WholeTag.Replace(rich, "");
  }
}