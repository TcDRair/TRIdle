using System.IO;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Logics.Serialization
{
  public abstract class LoaderBase
  {
    public abstract IEnumerator Load();
    public abstract IEnumerator Save();

    protected string FilePath => Application.streamingAssetsPath;

    public static T Deserialize<T>(string path) {
      try {
        using var stream = new FileStream(path, FileMode.Open);
        return JsonSerializer.Deserialize<T>(stream, GlobalConstants.JsonSerializerOption);
      }
      catch { return default; }
    }
    public static bool TryDeserialize<T>(string path, out T data)
      => (data = Deserialize<T>(path)) is not null;

    public static bool TrySerialize<T>(string path, T data) {
      if (Directory.Exists(Path.GetDirectoryName(path)) is false)
        Directory.CreateDirectory(Path.GetDirectoryName(path));

      using var stream = new FileStream(path, FileMode.Create);
      try {
        JsonSerializer.Serialize(stream, data, GlobalConstants.JsonSerializerOption);
        return true;
      }
      catch { return false; }
    }
  }
}