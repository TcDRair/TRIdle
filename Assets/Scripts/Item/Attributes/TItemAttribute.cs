using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Item.Attributes
{
  public class Attributes : ObjectLoader
  {
    //TODO Indexer or Dictionary for quick access
    //Caution : Different enum values
    public static readonly Dictionary<Attr, TItemAttribute> List = new();
    public override IEnumerator Load() {
      foreach (var attr in Resources.LoadAll<TItemAttribute>(PathHolder.Resources.Attributes)) {
        Attr value;
        try { value = Enum.Parse<Attr>(attr.Name); }
        catch { continue; } // If the attribute is not in the enum
        List.Add(value, attr);
        if (Elapsed) yield return null;
      }
    }
  }

  // Internal values and Public indicators for them
  [CreateAssetMenu(fileName = "ItemAttribute", menuName = "TRIdle/Item/Attribute")]
  public class TItemAttribute : ScriptableObject
  {
    public string Name;
    public string Description;
    public Sprite Sprite;

    public TItemAttribute Parent;

    public bool Is(TItemAttribute attribute) =>
      this == attribute ||
      (Parent != null && Parent.Is(attribute) == true);
    public bool Is(Attr attr) =>
      Attributes.List.TryGetValue(attr, out var attribute) && attribute.Is(this);

    #region Operators
    public static bool operator ==(TItemAttribute a, TItemAttribute b)
      => a.Name == b.Name;
    public static bool operator !=(TItemAttribute a, TItemAttribute b)
      => a.Name != b.Name;
    public override bool Equals(object obj)
      => obj is TItemAttribute attribute && this == attribute;
    public override int GetHashCode() => Name.GetHashCode();
    #endregion
  }
}
