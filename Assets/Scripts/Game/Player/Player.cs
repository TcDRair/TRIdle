using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game
{
  using UI;
  using Skill;
  using PlayerInternal;

  using Logics.Math;
  using Logics.Extensions;
  using Logics.Serialization;

  public class Player : LoaderBase
  {
    static Player m_instance;
    public static Player Instance => m_instance ??= new();

    public PlayerData Data { get; private set; } = new();


    private static IEnumerable<SkillBase> Skills => typeof(Skills)
      .GetProperties(BindingFlags.Public | BindingFlags.Static)
      .Where(property => property.PropertyType.IsSubclassOf(typeof(SkillBase)))
      .Select(property => property.GetValue(null) as SkillBase); // null(owner) = static
    public override IEnumerator Load() {
      this.Log($"Loading player data...");
      if (TryDeserializeDynamic($"{FilePath}/player.json", out var node)) {
        Data.FromJson(node["player"]);

        var skillNode = node["skills"];
        foreach (var skill in Skills)
          skill.LoadData(skillNode[skill.ID.ToString()]);
      }
      yield break;
    }

    public override IEnumerator Save() {
      this.Log($"Saving player data...");
      JsonObject node = new(), skillNode = new();
      
      foreach (var skill in Skills)
        skillNode[skill.ID.ToString()] = skill.SaveData();

      node["player"] = Data.ToJson();
      node["skills"] = skillNode;

      if (TrySerializeDynamic($"{FilePath}/player.json", node) is false) 
        this.Log($"Failed to save player data.");

      yield break;
    }

    #region Skill Actions
    PlayerMono m_mono;
    PlayerMono Mono {
      get {
        if (m_mono == null) {
          m_mono = new GameObject("PlayerMono").AddComponent<PlayerMono>();
          UnityEngine.Object.DontDestroyOnLoad(m_mono.gameObject);
        }
        return m_mono;
      }
    }

    // Start Action Delay, but if the same action is focused, stop the delay instead.
    public void FocusAction(ActionBase action) {
      Mono.StartActionDelay(Data.CurrentAction = (Data.CurrentAction == action) ? null : action);
      UI_MainSceneController.Instance.Action_Focus(action);
    }

    #endregion
  }

  public record PlayerData
  {
    public SkillBase CurrentSkill;
    public ActionBase CurrentAction;

    public RFloat ActionSpeed = new(1);

    public JsonObject ToJson() => new() {
      ["Placeholder"] = "Placeholder"
    };
    public void FromJson(JsonNode node) {
      node["Placeholder"] = "Placeholder";
    }
  }
}