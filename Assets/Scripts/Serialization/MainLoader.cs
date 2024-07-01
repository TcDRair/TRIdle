using System.IO;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

namespace TRIdle
{
  using Game;
  using Knowledge;

  public abstract class IRepository<T, U>
  {
    protected IRepository() { }
    protected Dictionary<T, U> instRepo = new();
    public int Count => instRepo.Count;
    public IEnumerable<T> Keys => instRepo.Keys;
    public IEnumerable<U> Values => instRepo.Values;

    public virtual U GetData(T key)
      => instRepo[key];
    public virtual bool TryGetData(T key, out U data)
      => instRepo.TryGetValue(key, out data);

    public virtual void Load(Stream stream)
    {
      try
      {
        instRepo = JsonSerializer.Deserialize<Dictionary<T, U>>(stream, Const.JsonSerializerOption);
      }
      catch
      {
        Debug.LogWarning($"Failed to load {typeof(U).Name} data. Creating new one...");
        instRepo = new();
      }
      finally
      {
        stream.Close();
      }
    }
    public virtual void Save(Stream stream)
      => JsonSerializer.Serialize(stream, instRepo, Const.JsonSerializerOption);

    // public abstract void ReadMarkdown(Stream stream);
    // public abstract void WriteMarkdown(Stream stream);
  }

  public class MainLoader : MonoBehaviour
  {
    string FilePath => Application.persistentDataPath;

    public TextMeshProUGUI progressText;

    void Awake()
    {
      // Load Player Data
      using var playerDataStream = new FileStream(FilePath + "/player.json", FileMode.Open);
      if (Player.IsLoaded is false) Player.Serializer.Load(playerDataStream);
      else { } // ...How?

      // Load All keywords
      using var keywordDataStream = new FileStream(FilePath + "/keywords.json", FileMode.Open);
      RP_Keyword.Instance.Load(keywordDataStream);

      // Load All Knowledge
      using var knowledgeDataStream = new FileStream(FilePath + "/knowledge.json", FileMode.Open);
      RP_Knowledge.Instance.Load(knowledgeDataStream);
    }

    void Start()
    {
      // DEBUG
      StartCoroutine(DebugLog());
    }

    IEnumerator DebugLog()
    {
      yield return new WaitForSeconds(1);
      Keyword.None.TryGenKnowledgeInfo(out var data);
      Debug.Log(data?.GetDescription());
    }

    void OnDestroy()
    {
      // Save Player Data
      using var playerDataStream = new FileStream(FilePath + "/player.json", FileMode.Create);
      Player.Serializer.Save(playerDataStream);

      // Save All keywords
      using var keywordDataStream = new FileStream(FilePath + "/keywords.json", FileMode.Create);
      RP_Keyword.Instance.Save(keywordDataStream);

      // Save All Knowledge
      using var knowledgeDataStream = new FileStream(FilePath + "/knowledge.json", FileMode.Create);
      RP_Knowledge.Instance.Save(knowledgeDataStream);
    }
  }
}