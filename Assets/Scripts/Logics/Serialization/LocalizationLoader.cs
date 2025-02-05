using System.IO;
using System.Collections;
using System.Text.Json;

using UnityEngine;

using System.Collections.Generic;

namespace TRIdle.Logics.Serialization
{
  using Extensions;

  public class LocalizationLoader : LoaderBase
  {
    public static LocalizationLoader Instance { get; private set; }
    void Awake() {
      if (Instance != null) Destroy(Instance.gameObject); // Always keep the latest instance
      Instance = this;
    }

    private Dictionary<string, string> languages;
    public override IEnumerator Load() {
      this.Log($"Loading localization files...");

      var path = Application.streamingAssetsPath + "/Localizations/";

      // Load language list
      if (TryDeserialize(path + "lists.json", out languages) is false) {
        this.Log("The language list file is not found. Localization will not be loaded.");
        yield break;
      }
      // Check each language files
      foreach (var (lang, name) in languages) {
        // Try to load each language files
        var langPath = $"{path}/{lang}/";
        foreach (var file in m_requiredFiles.Values)
          if (File.Exists(langPath + file) is false)
            this.Log($"The required file({file}) is not found for language({lang}). Some texts may not be displayed correctly.");
      }

      // Load selected (or default) language
      var selectedLang = PlayerPrefs.GetString("Language", "en");
      if (languages.ContainsKey(selectedLang) is false) {
        this.Log($"The selected language({selectedLang}) is not found. Default language(en) will be used.");
        selectedLang = "en";
      }
      var slPath = $"{path}/{selectedLang}/";
      if (TryDeserialize(slPath + m_requiredFiles["menu"], out Texts.Menu menu))
        Text.Current.Menu = menu;
      if (TryDeserialize(slPath + m_requiredFiles["settings"], out Texts.Settings settings))
        Text.Current.Settings = settings;
      // Add more files here
      
    }

    private readonly Dictionary<string, string> m_requiredFiles = new() {
      { "menu", "menu.json" },
      { "settings", "settings.json" },
    };
  }
}