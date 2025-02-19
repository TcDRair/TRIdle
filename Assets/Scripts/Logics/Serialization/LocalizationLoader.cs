using System.IO;
using System.Collections;
using System.Text.Json;

using UnityEngine;

using System.Collections.Generic;

namespace TRIdle.Logics.Serialization
{
  using Extensions;
  using TRIdle.Localization;

  public class LocalizationLoader : LoaderBase
  {
    static LocalizationLoader m_instance;
    public static LocalizationLoader Instance => m_instance ??= new();

    private readonly string[] files = { "title.json", "settings.json" };
    private Dictionary<string, string> languages;

    public override IEnumerator Load() {
      this.Log($"Loading localization files...");

      var path = $"{FilePath}/Localizations/";

      // Load language list
      if (TryDeserialize(path + "lists.json", out languages) is false) {
        this.Log("The language list file is not found. Localization will not be loaded.");
        yield break;
      }
      // Check each language files
      foreach (var (lang, name) in languages)
        FindMissingFiles(lang);

      // Load selected (or default) language
      yield return LoadTexts("ko");

      void FindMissingFiles(string lang) {
        var langPath = $"{path}/{lang}/";
        foreach (var file in files)
          if (File.Exists(langPath + file) is false)
            this.Log($"The required file({file}) is not found for language({lang}). Some texts may not be displayed correctly.");
      }
    }

    public IEnumerator LoadTexts(string lang) {
      if (languages.ContainsKey(lang) is false) {
        this.Log($"The selected language({lang}) is not found. Default language(en) will be used.");
        lang = "ko";
      }

      var path = $"{FilePath}/Localizations/{lang}/";
      TextLocale.Current = new() {
        Title = Deserialize<Text_Title>(path + files[0]) ?? new(),
        Settings = Deserialize<Text_Settings>(path + files[1]) ?? new(),
        // Add more files here
      };

      yield return null;
    }


    #if UNITY_EDITOR
    // Localization files are not supposed to be saved during runtime
    // Use this only for debugging purposes
    public override IEnumerator Save() {
      this.Log($"Saving localization files...");
      
      var path = $"{FilePath}/Localizations/ko";
      TrySerialize(path + "/title.json", TextLocale.Current.Title);
      TrySerialize(path + "/settings.json", TextLocale.Current.Settings);

      this.Log("Localization files are saved.");
      yield return null;
    }
    #endif
  }
}