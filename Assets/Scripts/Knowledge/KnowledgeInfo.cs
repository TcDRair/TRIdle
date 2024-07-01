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
  /// <summary>
  /// Knowledge is a collection of information about the game world.<br/>
  /// All knowledge class has a prefix of "K_".
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
  public abstract class KnowledgeInfo
  {
    [JsonInclude] protected string ImagePath { get; set; } = Const.SPRITE_PLACEHOLDER_PATH;
    // TODO : Separate description into multiple parts.
    [JsonInclude] public string FlatDescription { get; set; }

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

    [JsonIgnore] readonly Regex keywordPattern = new(@"\{(?<Type>\w+):(?<Name>\w+)\}");
    [JsonIgnore] MatchCollection keywordMatches;
    [JsonIgnore] readonly Dictionary<string, KeywordBase> keywordList = new();
    [JsonIgnore] public IEnumerable<Keyword> Keyword => keywordList.Values.Select(x => x.Key);
    [JsonIgnore] public IEnumerable<KeywordBase> AssociatedKeywords => keywordList.Values;
    [JsonIgnore] public Sprite MainImage { get; private set; }

    public override string ToString()
      => GetDescription();
    }

  /// <summary>
  /// Contains a main keyword which enables the knowledge to be searchable / categorizable.
  /// </summary>
  /// <typeparam name="T">Main Keyword Type</typeparam>
  public abstract class KnowledgeInfo<T> : KnowledgeInfo where T : KeywordBase
  {
    [JsonIgnore] public T MainKeyword { get; set; }

    public KnowledgeInfo(T mainKeyword)
    {
      MainKeyword = mainKeyword;
    }

    public override string ToString()
      => $"{MainKeyword.Key} : {GetDescription()}";
  }

  #region Derived Definitions
  // * Knowledge is a collection of information about the game world.
  // * All knowledge class has a prefix of "K_".

  // Real targets: Creature / Item / Incident / Location
  public class KI_Creature : KnowledgeInfo<Kw_Creature>
  {
    public KI_Creature(Kw_Creature key) : base(key) { }
    public KI_Creature[] Subspecies { get; set; }
    public KI_Effect[] NaturalEffects { get; set; }
    // No plan to implement all hierarchy of creatures (for now)
  }

  public class KI_Item : KnowledgeInfo<Kw_Item>
  {
    public KI_Item(Kw_Item key) : base(key) { }
    public KI_Property[] Properties { get; set; }
  }

  public class KI_Incident : KnowledgeInfo<Kw_Incident>
  {
    public KI_Incident(Kw_Incident key) : base(key) { }
    public KI_Creature[] InvolvedCreatures { get; set; }
    public KI_Item[] InvolvedItems { get; set; }
    public KI_Location[] Locations { get; set; }
  }

  public class KI_Location : KnowledgeInfo<Kw_Location>
  {
    public KI_Location(Kw_Location key) : base(key) { }
    public KI_Creature[] Inhabitants { get; set; }
    public KI_Item[] Items { get; set; }
  }

  // Virtual targets: Trait + Effect (Creature related) / Property (Item related)
  public class KI_Trait : KnowledgeInfo<Kw_Trait>
  {
    public KI_Trait(Kw_Trait key) : base(key) { }
  }

  public class KI_Effect : KnowledgeInfo<Kw_Effect>
  {
    public KI_Effect(Kw_Effect key) : base(key) { }
  }

  public class KI_Property : KnowledgeInfo<Kw_Property>
  {
    public KI_Property(Kw_Property key) : base(key) { }
  }
  // Meta targets: Info / Tutorial / Guide
  // TODO...
  #endregion
}