


namespace TRIdle.Keywords
{
  public class Keyword
  {
    public string Name { get; set; }
  }

  public class ItemKeyword : Keyword
  {
    // All Items have this keyword.
    // It is used as category for items.
    // Example : "Weapon", "Armor", "Consumable", etc.
  }

  public class ActionKeyword : Keyword
  {
    // Used for Skill / Action (mostly).
    // Some keywords are same as Skill/Action name, but most are used to description to explain those.
    // Almost all keywords of other categories also use these.
    // Example : "WoodCutting", "Instant", "Aura", etc.
  }

  public class TraitKeyword : Keyword
  {
    // All Traits have its own keyword, and replace its name with this.
    // Example : Trait: "Agile" itself is a keyword.
  }

  public class CreatureKeyword : Keyword
  {
    // All Creatures have its own keyword, and replace its name with this.
    // Example : Creature: "Goblin" itself is a keyword.
  }
  public class LocationKeyword : Keyword
  {
    // All Locations have its own keyword, and replace its name with this.
    // Example : Location: "Ve-ol Island" itself is a keyword.
  }
  public class IncidentKeyword : Keyword
  {
    // All Incidents have its own keyword, and replace its name with this.
    // Example : Incident: "Ve-ol Ancient Curse" itself is a keyword.
  }
}