using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using UnityEngine;

namespace TRIdle {
  using Knowledge;

  namespace Knowledge {
    public class KnowledgeConverter<T> : JsonConverter<T> {
      public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new NotImplementedException();
      }

      public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
        throw new NotImplementedException();
      }
    }


    public class RP_Knowledge : IRepository<Keyword, IKnowledgeInfo> {
      private RP_Knowledge() { }
      public static RP_Knowledge Instance { get; } = new();

      public bool TryAddDefault() => instRepo.TryAdd(Keyword.None, new KI_Item()); // Default Type is Item
    }

    public class RP_Keyword : IRepository<Keyword, KeywordBase> {
      private RP_Keyword() { }
      public static RP_Keyword Instance { get; } = new();
    }
  }

  public static partial class Extensions {
    public static KeywordBase GetKeywordInfo(this Keyword key)
      => RP_Keyword.Instance.GetData(key);
    public static bool TryGetKeywordInfo(this Keyword key, out KeywordBase keyword)
      => RP_Keyword.Instance.TryGetData(key, out keyword);

    public static IKnowledgeInfo GetKnowledgeInfo(this Keyword key)
      => RP_Knowledge.Instance.GetData(key);
    public static bool TryGenKnowledgeInfo(this Keyword key, out IKnowledgeInfo knowledge)
      => RP_Knowledge.Instance.TryGetData(key, out knowledge);
  }
}