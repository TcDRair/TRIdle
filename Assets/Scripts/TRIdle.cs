using UnityEngine;
using UnityEditor;

namespace TRIdle
{
  public class Const
  {
    public const string PLACEHOLDER = "PlaceHolder";
  }


  public static class Ext
  {
    public static void DestroyAllChildren(this GameObject obj) => DestroyAllChildren(obj.transform);
    public static void DestroyAllChildren(this Transform tr) {
      for (int i = tr.childCount - 1; i >= 0; i--) {
        Object.Destroy(tr.GetChild(i).gameObject);
      }
    }

    /// <summary>
    /// Rounds the value to the nearest integer, with a probability of rounding up.
    /// </summary>
    public static int ProbabilisticRound(this float value) => (int)value + (Random.value < value % 1 ? 1 : 0);
    /// <summary>A shorthand for <see cref="ProbabilisticRound(float)"/>.</summary>
    public static int PRound(this float value) => ProbabilisticRound(value);
  }
}