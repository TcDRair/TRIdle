using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle
{
  using Knowledge;

  namespace Knowledge
  {
    public class RP_Knowledge : IRepository<Keyword, KnowledgeInfo>
    {
      private RP_Knowledge() { }
      public static RP_Knowledge Instance { get; } = new();

      public override void Load(Stream stream)
      {
        base.Load(stream);
        
        // Check 
        foreach (var key in Enum.GetValues(typeof(Keyword)).Cast<Keyword>())
          if (instRepo.TryGetValue(key, out _) is false)
            Debug.LogWarning($"Keyword {key} was not loaded properly.");

        // DEBUG
        instRepo[Keyword.None] = new KI_Trait(Keyword.None.GetKeywordInfo() as Kw_Trait)
        {
          FlatDescription = "This is a test description. {Trait:None} is associated keyword of this.",
        };
      }
    }

    public class RP_Keyword : IRepository<Keyword, KeywordBase>
    {
      private RP_Keyword() { }
      public static RP_Keyword Instance { get; } = new();

      public override void Load(Stream stream)
      {
        base.Load(stream);

        // Check 
        foreach (var key in Enum.GetValues(typeof(Keyword)).Cast<Keyword>())
          if (instRepo.TryGetValue(key, out _) is false)
            Debug.LogWarning($"Keyword {key} was not loaded properly.");

        // DEBUG
        instRepo[Keyword.None] = new Kw_Trait() { Key = Keyword.None, Description = "This is a placeholder keyword." };
      }
    }
  }

  public static partial class Extensions
  {
    public static KeywordBase GetKeywordInfo(this Keyword key)
      => RP_Keyword.Instance.GetData(key);
    public static bool TryGetKeywordInfo(this Keyword key, out KeywordBase keyword)
      => RP_Keyword.Instance.TryGetData(key, out keyword);

    public static KnowledgeInfo GetKnowledgeInfo(this Keyword key)
      => RP_Knowledge.Instance.GetData(key);
    public static bool TryGenKnowledgeInfo(this Keyword key, out KnowledgeInfo knowledge)
      => RP_Knowledge.Instance.TryGetData(key, out knowledge);
  }
}