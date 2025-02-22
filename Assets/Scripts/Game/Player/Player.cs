using System;
using System.Linq;
using System.Reflection;
using System.Collections;

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

    // ... PlayerData에 스킬도 다 넣어버릴까 그냥
    public override IEnumerator Load() {
      this.Log($"Loading player data...");
      if (TryDeserializeDynamic($"{FilePath}/player.json", out var node)) {
        Data = node["player"].GetValue<PlayerData>();
        // Skill Progress = node["skills"].GetValue<sth>();
      }
      yield break;

      IEnumerator LoadSkill() {
        var skills = typeof(Skills)
          .GetProperties(BindingFlags.Public | BindingFlags.Static)
          .Where(property => property.PropertyType.IsSubclassOf(typeof(SkillBase)))
          .Select(property => property.GetValue(null) as SkillBase);

        foreach (var skill in skills) {
          // Load the skill.
          // the skill would be serialized to json as List<SkillBase>.
          // During the deserialization, it will be Dictionary<string, SkillBase>.
          // The key is the skill ID.
        }

        yield break;
      }
    }

    public override IEnumerator Save() {
      this.Log($"Saving player data...");
      if (TrySerialize($"{FilePath}/player.json", Data))
        yield break;
      this.Log($"Failed to save player data.");
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
      UI_MainSceneController.Instance.FocusAction(action);
    }

    #endregion
  }

  public record PlayerData
  {
    public SkillBase CurrentSkill;
    public ActionBase CurrentAction;

    public RFloat ActionSpeed = new(1);
  }
}