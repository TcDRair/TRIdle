using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using UnityEngine;
namespace TRIdle.Knowledge {
  public enum KeywordType {
    Item, // Meta Information
    Trait, // Applied to Creature
    Effect, // Applied to Creature. Is there any better name?
    Property, // Applied to Item
    Creature,
    Location,
    Incident
  }

  #region Derived Attributes
  [JsonDerivedType(typeof(Kw_Item), "Item")]
  [JsonDerivedType(typeof(Kw_Trait), "Trait")]
  [JsonDerivedType(typeof(Kw_Effect), "Effect")]
  [JsonDerivedType(typeof(Kw_Property), "Property")]
  [JsonDerivedType(typeof(Kw_Creature), "Creature")]
  [JsonDerivedType(typeof(Kw_Location), "Location")]
  [JsonDerivedType(typeof(Kw_Incident), "Incident")]
  #endregion
  public abstract class KeywordBase {
    public Keyword Key { get; set; }
    public string Description { get; set; }
    public abstract KeywordType Type { get; }

    public override string ToString()
      => $"[{Key}({Type}): \"{Description}\"]";
  }

  public class Kw_Item : KeywordBase {
    // All Items have this keyword.
    // It is used as category for items.
    // Example : "Weapon", "Armor", "Consumable", etc.
    public override KeywordType Type => KeywordType.Item;
  }
  public class Kw_Trait : KeywordBase {
    // All Traits have its own keyword, and replace its name with this.
    // Example : Trait: "Agile" itself is a keyword.
    public override KeywordType Type => KeywordType.Trait;
  }
  public class Kw_Effect : KeywordBase {
    // All Effects have its own keyword, and replace its name with this.
    // Example : Effect: "Poison" itself is a keyword.
    public override KeywordType Type => KeywordType.Effect;
  }
  public class Kw_Property : KeywordBase {
    // All Properties have its own keyword, and replace its name with this.
    // Example : Property: "Durability" itself is a keyword.
    public override KeywordType Type => KeywordType.Property;
  }
  public class Kw_Creature : KeywordBase {
    // All Creatures have its own keyword, and replace its name with this.
    // Example : Creature: "Goblin" itself is a keyword.
    public override KeywordType Type => KeywordType.Creature;
  }
  public class Kw_Location : KeywordBase {
    // All Locations have its own keyword, and replace its name with this.
    // Example : Location: "Ve-ol Island" itself is a keyword.
    public override KeywordType Type => KeywordType.Location;
  }
  public class Kw_Incident : KeywordBase {
    // All Incidents have its own keyword, and replace its name with this.
    // Example : Incident: "Ve-ol Ancient Curse" itself is a keyword.
    public override KeywordType Type => KeywordType.Incident;
  }
}