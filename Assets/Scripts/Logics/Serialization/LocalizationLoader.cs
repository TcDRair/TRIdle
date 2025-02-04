using System.IO;
using System.Collections;
using System.Text.Json;

using UnityEngine;

using System.Collections.Generic;

namespace TRIdle.Serialization
{
  public class LocalizationLoader : MonoBehaviour, ILoader
  {
    public static LocalizationLoader Instance { get; private set; }
    void Awake()
    {
      if (Instance != null) Destroy(Instance.gameObject); // Always keep the latest instance
      Instance = this;

      StartCoroutine(Load());
    }

    public IEnumerator Load()
    {
      // for debug
      yield return new WaitForSeconds(3f);

      var path = Application.streamingAssetsPath + "/Localizations/";

      // Load language list
      var languages = Deserialize<Dictionary<string, string>>(path + "lists.json");

      foreach (var (lang, name) in languages)
      {
        // Try to load each language files
        var langPath = $"{path}/{lang}/";
        foreach (var file in m_requiredFiles)
        {
          if (File.Exists(langPath + file) is false)
          {
            Debug.Log($"Loading {lang} {file}");
            var text = Deserialize<Text>(langPath + file);
            // Do something with text
          }
        }
      }

      yield break;
    }

    private readonly List<string> m_requiredFiles = new() {
      "menu.json",
      "settings.json",
    };

    private T Deserialize<T>(string path)
    {
      // use System.Text.Json
      using var stream = new FileStream(path, FileMode.Open);
      using var reader = new StreamReader(stream);
      return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
    }
  }
}