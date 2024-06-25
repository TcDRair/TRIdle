


namespace TRIdle.Keywords
{
  public enum KeywordType
  {
    ItemCategory,
    Action,
    Trait,
    Creature,
    Location,
    Incident
  }
  public abstract class Keyword
  {
    public string Name { get; set; }
    public abstract KeywordType Type { get; }
  }

  public class ItemKeyword : Keyword
  {
    // All Items have this keyword.
    // It is used as category for items.
    // Example : "Weapon", "Armor", "Consumable", etc.
    public override KeywordType Type => KeywordType.ItemCategory;
  }

  public class ActionKeyword : Keyword
  {
    // Used for Skill / Action (mostly).
    // Some keywords are same as Skill/Action name, but most are used to description to explain those.
    // Almost all keywords of other categories also use these.
    // Example : "WoodCutting", "Instant", "Aura", etc.
    public override KeywordType Type => KeywordType.Action;
  }

  public class TraitKeyword : Keyword
  {
    // All Traits have its own keyword, and replace its name with this.
    // Example : Trait: "Agile" itself is a keyword.
    public override KeywordType Type => KeywordType.Trait;
  }

  public class CreatureKeyword : Keyword
  {
    // All Creatures have its own keyword, and replace its name with this.
    // Example : Creature: "Goblin" itself is a keyword.
    public override KeywordType Type => KeywordType.Creature;
  }
  public class LocationKeyword : Keyword
  {
    // All Locations have its own keyword, and replace its name with this.
    // Example : Location: "Ve-ol Island" itself is a keyword.
    public override KeywordType Type => KeywordType.Location;
  }
  public class IncidentKeyword : Keyword
  {
    // All Incidents have its own keyword, and replace its name with this.
    // Example : Incident: "Ve-ol Ancient Curse" itself is a keyword.
    public override KeywordType Type => KeywordType.Incident;
  }
}