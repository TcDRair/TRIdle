using System.IO;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

namespace TRIdle.Logics.Serialization
{
  using Game;
  using Extensions;

  public class MainLoader : LoaderBase
  {
    public static MainLoader Instance { get; private set; }
    void Awake() {
      if (Instance != null) Destroy(Instance.gameObject); // Always keep the latest instance
      Instance = this;
    }

    string FilePath => Application.streamingAssetsPath;

    public TextMeshProUGUI progressText;

    void Start() {
      StartCoroutine(Load());
    }

    public override IEnumerator Load() {
      // Load Player Data
      this.Log("Loading Player Data");
      using var playerDataStream = new FileStream(FilePath + "/player.json", FileMode.Open);
      if (Player.IsLoaded is false) Player.Serializer.Load(playerDataStream);
      
      // Load Localization Data
      this.Log("Loading Localization Data");
      yield return LocalizationLoader.Instance.Load();
    }

    void OnDestroy() {
      // Save Player Data
      using var playerDataStream = new FileStream(FilePath + "/player.json", FileMode.Create);
      Player.Serializer.Save(playerDataStream);
    }
  }
}