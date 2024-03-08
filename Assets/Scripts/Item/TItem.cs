

using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Item
{
  public class TItem: ScriptableObject {
    public string itemName;
    public string tooltip;
    public struct Info { // TODO: Required Attributes <=> Info
      public Quality quality;
      public Rarity rarity;
      public int level;
      public int value;
    } public Info info;
    public List<Tag> tags;
  }

  /// <summary>상대적인 품질을 나타냅니다.</summary>
  public enum Quality {
    /// <summary>Low quality</summary>
    Poor,
    /// <summary>Average quality, but negative effect on the item</summary>
    Substandard,
    /// <summary>Average quality</summary>
    Standard,
    /// <summary>Average quality, but positive effect on the item</summary>
    Superior,
    /// <summary>High quality, but negative effect on the item</summary>
    Exemplary,
    /// <summary>High quality</summary>
    Exquisite,
    /// <summary>High quality, but positive effect on the item</summary>
    Exceptional,
    /// <summary>Highest quality</summary>
    Premium
  }
  /// <summary>절대적인 희귀도를 나타냅니다.</summary>
  public enum Rarity {
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
  }
  public struct Affix
  {
    public string text;
    public int order;
    //TODO Type ({1} {2} {3} baseName {4} {5})
  }
}
