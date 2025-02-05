using System.IO;
using System.Text.Json;
using System.Collections;

using UnityEngine;

namespace TRIdle.Logics.Serialization
{
  public abstract class LoaderBase : MonoBehaviour
  {
    public abstract IEnumerator Load();

    public static bool TryDeserialize<T>(string path, out T data) {
      if (File.Exists(path) is false) {
        data = default;
        return false;
      }

      using var stream = new FileStream(path, FileMode.Open);
      try {
        data = JsonSerializer.Deserialize<T>(stream, Const.JsonSerializerOption);
        return true;
      }
      catch {
        data = default;
        return false;
      }
    }
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