using System.IO;
using System.Collections;
using System.Text.Json;

using UnityEngine;

using System.Collections.Generic;

namespace TRIdle.Logics.Serialization
{
  using Extensions;
  using TRIdle.Texts;

  public class LocalizationLoader : LoaderBase
  {
    public static LocalizationLoader Instance { get; private set; }
    protected override void Awake() {
      base.Awake();
      if (Instance != null) Destroy(Instance.gameObject); // Always keep the latest instance
      Instance = this;
    }

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
        foreach (var file in fileNames.Values)
          if (File.Exists(langPath + file) is false)
            this.Log($"The required file({file}) is not found for language({lang}). Some texts may not be displayed correctly.");
      }
    }

    public override IEnumerator Save() {
      // Localization files are not supposed to be saved during runtime
      yield break;
    }

    public IEnumerator LoadTexts(string lang) {
      if (languages.ContainsKey(lang) is false) {
        this.Log($"The selected language({lang}) is not found. Default language(en) will be used.");
        lang = "en";
      }

      var path = $"{FilePath}/Localizations/{lang}/";
      Text.Current = new() {
        Title = Deserialize<Title>(path + fileNames[RFTypes.Title]) ?? new(),
        Settings = Deserialize<Settings>(path + fileNames[RFTypes.Settings]) ?? new(),
        // Add more files here
      };
      
      yield return null;
    }

    private readonly Dictionary<RFTypes, string> fileNames = new() {
      { RFTypes.Title, "title.json" },
      { RFTypes.Settings, "settings.json" },
      // And so on
    };
    private enum RFTypes { Title, Settings, Menu, Skills, Actions, Items, Dialogues, Notifications, }
  }
}