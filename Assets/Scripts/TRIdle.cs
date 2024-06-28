using UnityEngine;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEditor;

namespace TRIdle
{
  public class Const
  {
    public const string PLACEHOLDER = "PlaceHolder";
    public const string SPRITE_PLACEHOLDER_PATH = "Sprites/PlaceHolder";
    public static readonly JsonSerializerOptions JsonSerializerOption = new()
    {
      WriteIndented = true,
      IgnoreReadOnlyProperties = true,
      ReferenceHandler = ReferenceHandler.IgnoreCycles,
      NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };
  }


  public static partial class Extensions
  {
    public static void DestroyAllChildren(this GameObject obj) => DestroyAllChildren(obj.transform);
    public static void DestroyAllChildren(this Transform tr)
    {
      for (int i = tr.childCount - 1; i >= 0; i--)
      {
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