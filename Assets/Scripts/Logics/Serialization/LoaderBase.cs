using System.IO;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Logics.Serialization
{
  public abstract class LoaderBase : MonoBehaviour
  {
    public abstract IEnumerator Load();
    public abstract IEnumerator Save();

    protected static List<LoaderBase> m_loaders = new();
    /// <summary>반드시 <see langword="base.Awake()"/>를 호출해야 작동합니다.</summary>
    protected virtual void Awake() => m_loaders.Add(this);

    public static IEnumerator LoadAll() {
      foreach (var l in m_loaders)
        yield return l.Load();
    }

    protected string FilePath => Application.streamingAssetsPath;

    public static T Deserialize<T>(string path) {
      try {
        using var stream = new FileStream(path, FileMode.Open);
        return JsonSerializer.Deserialize<T>(stream, Const.JsonSerializerOption);
      }
      catch {
        return default;
      }
    }
    public static bool TryDeserialize<T>(string path, out T data)
      => (data = Deserialize<T>(path)).Equals(default) is false;

    public static bool TrySerialize<T>(string path, T data) {
      if (Directory.Exists(Path.GetDirectoryName(path)) is false)
        Directory.CreateDirectory(Path.GetDirectoryName(path));

      using var stream = new FileStream(path, FileMode.Create);
      try {
        JsonSerializer.Serialize(stream, data, Const.JsonSerializerOption);
        return true;
      }
      catch {
        return false;
      }
    }
  }
}