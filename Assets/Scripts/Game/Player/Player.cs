using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections;

using UnityEngine;

namespace TRIdle.Game
{
  using Skill;

  using Logics.Extensions;
  using Logics.Serialization;

  public class Player : LoaderBase
  {
    public static Player Instance { get; private set; }
    protected override void Awake() {
      base.Awake();
      if (Instance != null) Destroy(Instance.gameObject); // Always keep the latest instance
      Instance = this;
    }

    public PlayerData Data { get; private set; }
    public override IEnumerator Load() {
      this.Log($"Loading player data...");
      if (TryDeserialize($"{FilePath}/player.json", out PlayerData data))
        Data = data;
      yield break;
    }

    public override IEnumerator Save() {
      this.Log($"Saving player data...");
      if (TrySerialize($"{FilePath}/player.json", Data))
        yield break;
      this.Log($"Failed to save player data.");
    }
  }

  public record PlayerData
  {

  }
}