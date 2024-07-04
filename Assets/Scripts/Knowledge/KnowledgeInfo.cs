using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

using UnityEngine;

namespace TRIdle.Knowledge {
  [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
  #region Derived Attributes
  [JsonDerivedType(typeof(KI_Creature), "Creature")]
  [JsonDerivedType(typeof(KI_Item), "Item")]
  [JsonDerivedType(typeof(KI_Incident), "Incident")]
  [JsonDerivedType(typeof(KI_Location), "Location")]
  [JsonDerivedType(typeof(KI_Trait), "Trait")]
  [JsonDerivedType(typeof(KI_Effect), "Effect")]
  [JsonDerivedType(typeof(KI_Property), "Property")]
  #endregion
  public interface IKnowledgeInfo {
    Keyword Keyword { get; set; }
    KeywordType Type { get; set; }
    string IconPath { get; set; }
    string FlatDescription { get; set; }

    IKnowledgeInfo Convert(KeywordType newType);
    string GetDescription();
    bool Same(IKnowledgeInfo other);
  }

  /// <summary>
  /// Knowledge is a collection of information about the game world.<br/>
  /// All knowledge class has a prefix "K_".
  /// </summary>
  public abstract class KnowledgeInfo<T> : IKnowledgeInfo where T : KeywordBase {
    #region Declarations
    [JsonInclude, JsonPropertyOrder(-4)] public Keyword Keyword { get; set; }
    [JsonInclude, JsonPropertyOrder(-3)] public KeywordType Type { get; set; }
    [JsonInclude, JsonPropertyOrder(-2)] public string IconPath { get; set; } = Const.SPRITE_PLACEHOLDER_PATH;
    [JsonInclude, JsonPropertyOrder(-1)] public string FlatDescription { get; set; } = "";
    [JsonIgnore] private readonly Regex keywordPattern = new(@"\{(?<Type>\w+):(?<Name>\w+)\}");
    [JsonIgnore] private MatchCollection keywordMatches;
    [JsonIgnore] private readonly Dictionary<string, KeywordBase> keywordList = new();
    [JsonIgnore] public IEnumerable<Keyword> Keywords => keywordList.Values.Select(x => x.Key);
    [JsonIgnore] public IEnumerable<KeywordBase> AssociatedKeywords => keywordList.Values;
    [JsonIgnore] public Sprite MainImage => Resources.Load<Sprite>(IconPath);
    #endregion

    public string GetDescription() {
      #region Generation phase (only once)
      keywordMatches ??= keywordPattern.Matches(FlatDescription);
      if (keywordMatches.Count == 0) return FlatDescription;
      if (keywordList.Count == 0) {
        foreach (Match match in keywordMatches) {
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
    public IKnowledgeInfo Convert(KeywordType newType) {
      IKnowledgeInfo newInfo = newType switch {
        KeywordType.Creature => new KI_Creature(),
        KeywordType.Item => new KI_Item(),
        KeywordType.Incident => new KI_Incident(),
        KeywordType.Location => new KI_Location(),
        KeywordType.Trait => new KI_Trait(),
        KeywordType.Effect => new KI_Effect(),
        KeywordType.Property => new KI_Property(),
        _ => throw new ArgumentException("Invalid keyword type.")
      };
      newInfo.Keyword = Keyword;
      newInfo.IconPath = IconPath;
      newInfo.FlatDescription = FlatDescription;
      return newInfo;
    }

    public virtual bool Same(IKnowledgeInfo other)
      => other is KnowledgeInfo<T> info
      && Keyword == info.Keyword
      && Type == info.Type
      && IconPath == info.IconPath
      && FlatDescription == info.FlatDescription;
    public override string ToString() => $"{Keyword} : {GetDescription()}";
  }

  #region Derived Definitions
  // * Knowledge is a collection of information about the game world.
  // * All knowledge class has a prefix of "K_".

  // Real targets: Creature / Item / Incident / Location
  public class KI_Creature : KnowledgeInfo<Kw_Creature> {
    public KI_Creature() { }
    public KI_Creature[] Subspecies { get; set; }
    public KI_Effect[] NaturalEffects { get; set; }
    // No plan to implement all hierarchy of creatures (for now)
  }

  public class KI_Item : KnowledgeInfo<Kw_Item> {
    public KI_Item() { Type = KeywordType.Item; }
    [JsonInclude] public List<KI_Property> Properties { get; set; } = new();

    public override bool Same(IKnowledgeInfo other)
      => base.Same(other)
      && other is KI_Item item
      && Properties.SequenceEqual(item.Properties);
  }

  public class KI_Incident : KnowledgeInfo<Kw_Incident> {
    public KI_Incident() { Type = KeywordType.Incident; }
    public KI_Creature[] InvolvedCreatures { get; set; }
    public KI_Item[] InvolvedItems { get; set; }
    public KI_Location[] Locations { get; set; }
  }

  public class KI_Location : KnowledgeInfo<Kw_Location> {
    public KI_Location() { Type = KeywordType.Location; }
    public KI_Creature[] Inhabitants { get; set; }
    public KI_Item[] Items { get; set; }
  }

  // Virtual targets: Trait + Effect (Creature related) / Property (Item related)
  public class KI_Trait : KnowledgeInfo<Kw_Trait> {
    public KI_Trait() { Type = KeywordType.Trait; }
  }

  public class KI_Effect : KnowledgeInfo<Kw_Effect> {
    public KI_Effect() { Type = KeywordType.Effect; }
  }

  public class KI_Property : KnowledgeInfo<Kw_Property> {
    public KI_Property() { Type = KeywordType.Property; }
  }
  // Meta targets: Info / Tutorial / Guide
  // TODO...
  #endregion
}