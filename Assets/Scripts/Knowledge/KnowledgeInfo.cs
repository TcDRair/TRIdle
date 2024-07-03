using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

using UnityEngine;

namespace TRIdle.Knowledge
{
  public interface IKnowledgeInfo
  {
    public string ImagePath { get; set; }
    public string FlatDescription { get; set; }

    public KeywordType Type { get; set; }
    public Keyword Keyword { get; set; }

    public IKnowledgeInfo Convert(KeywordType newType);

    public string GetDescription();
  }

  /// <summary>
  /// Knowledge is a collection of information about the game world.<br/>
  /// All knowledge class has a prefix "K_".
  /// </summary>
  #region Derived Attributes
  [JsonDerivedType(typeof(KI_Creature), "Creature")]
  [JsonDerivedType(typeof(KI_Item), "Item")]
  [JsonDerivedType(typeof(KI_Incident), "Incident")]
  [JsonDerivedType(typeof(KI_Location), "Location")]
  [JsonDerivedType(typeof(KI_Trait), "Trait")]
  [JsonDerivedType(typeof(KI_Effect), "Effect")]
  [JsonDerivedType(typeof(KI_Property), "Property")]
  #endregion
  public abstract class KnowledgeInfo<T> : IKnowledgeInfo where T : KeywordBase
  {
    #region Declarations
    [JsonInclude] public string ImagePath { get; set; } = Const.SPRITE_PLACEHOLDER_PATH;
    [JsonInclude] public string FlatDescription { get; set; } = "";
    [JsonInclude] public KeywordType Type { get; set; }
    [JsonInclude] public Keyword Keyword { get; set; }

    [JsonIgnore] private readonly Regex keywordPattern = new(@"\{(?<Type>\w+):(?<Name>\w+)\}");
    [JsonIgnore] private MatchCollection keywordMatches;
    [JsonIgnore] private readonly Dictionary<string, KeywordBase> keywordList = new();
    [JsonIgnore] public IEnumerable<Keyword> Keywords => keywordList.Values.Select(x => x.Key);
    [JsonIgnore] public IEnumerable<KeywordBase> AssociatedKeywords => keywordList.Values;
    [JsonIgnore] public Sprite MainImage => Resources.Load<Sprite>(ImagePath);
    #endregion

    public KnowledgeInfo(Keyword keyword) => Keyword = keyword;

    public string GetDescription()
    {
      #region Generation phase (only once)
      keywordMatches ??= keywordPattern.Matches(FlatDescription);
      if (keywordMatches.Count == 0) return FlatDescription;
      if (keywordList.Count == 0)
      {
        foreach (Match match in keywordMatches)
        {
          string type = match.Groups["Type"].Value; // "Keyword Type"
          if (Enum.TryParse<Keyword>(match.Groups["Name"].Value, out var name)) // "Keyword Name"
            if (name.TryGetKeywordInfo(out var keyword) && keyword.Type.ToString() == type)
              keywordList.Add(match.Value, keyword);
        }
      }
      #endregion

      string dynamicDescription = FlatDescription;
      foreach (var pair in keywordList)
        dynamicDescription = dynamicDescription.Replace(pair.Key, $"<color=black>{pair.Value}</color>");

      return dynamicDescription + "\n\nTodo : Seperate description into multiple parts.";
    }

    
    /// <summary>
    /// Converts this KnowledgeInfo object to a derived class.
    /// Common properties are copied to the new object.
    /// </summary>
    /// <param name="newType">New type of the object</param>
    /// <returns>Converted object</returns>
    public IKnowledgeInfo Convert(KeywordType newType)
    {
      IKnowledgeInfo newInfo = newType switch
      {
        KeywordType.Creature => new KI_Creature(Keyword),
        KeywordType.Item => new KI_Item(Keyword),
        KeywordType.Incident => new KI_Incident(Keyword),
        KeywordType.Location => new KI_Location(Keyword),
        KeywordType.Trait => new KI_Trait(Keyword),
        KeywordType.Effect => new KI_Effect(Keyword),
        KeywordType.Property => new KI_Property(Keyword),
        _ => throw new ArgumentException("Invalid keyword type.")
      };
      newInfo.ImagePath = ImagePath;
      newInfo.FlatDescription = FlatDescription;
      return newInfo;
    }

    public override string ToString() => $"{Keyword} : {GetDescription()}";
  }

  #region Derived Definitions
  // * Knowledge is a collection of information about the game world.
  // * All knowledge class has a prefix of "K_".

  // Real targets: Creature / Item / Incident / Location
  public class KI_Creature : KnowledgeInfo<Kw_Creature>
  {
    public KI_Creature(Keyword key) : base(key) { }
    public KI_Creature[] Subspecies { get; set; }
    public KI_Effect[] NaturalEffects { get; set; }
    // No plan to implement all hierarchy of creatures (for now)
  }

  public class KI_Item : KnowledgeInfo<Kw_Item>
  {
    public KI_Item(Keyword key) : base(key) { Type = KeywordType.Item; }
    public KI_Property[] Properties { get; set; }
  }

  public class KI_Incident : KnowledgeInfo<Kw_Incident>
  {
    public KI_Incident(Keyword key) : base(key) { Type = KeywordType.Incident; }
    public KI_Creature[] InvolvedCreatures { get; set; }
    public KI_Item[] InvolvedItems { get; set; }
    public KI_Location[] Locations { get; set; }
  }

  public class KI_Location : KnowledgeInfo<Kw_Location>
  {
    public KI_Location(Keyword key) : base(key) { Type = KeywordType.Location; }
    public KI_Creature[] Inhabitants { get; set; }
    public KI_Item[] Items { get; set; }
  }

  // Virtual targets: Trait + Effect (Creature related) / Property (Item related)
  public class KI_Trait : KnowledgeInfo<Kw_Trait>
  {
    public KI_Trait(Keyword key) : base(key) { Type = KeywordType.Trait; }
  }

  public class KI_Effect : KnowledgeInfo<Kw_Effect>
  {
    public KI_Effect(Keyword key) : base(key) { Type = KeywordType.Effect; }
  }

  public class KI_Property : KnowledgeInfo<Kw_Property>
  {
    public KI_Property(Keyword key) : base(key) { Type = KeywordType.Property; }
  }
  // Meta targets: Info / Tutorial / Guide
  // TODO...
  #endregion
}