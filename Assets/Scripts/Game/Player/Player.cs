using System;
using System.Collections;

using UnityEngine;

namespace TRIdle.Game
{
  using UI;
  using PlayerInternal;

  using Skill;

  using Logics.Extensions;
  using Logics.Serialization;

  public class Player : LoaderBase
  {
    static Player m_instance;
    public static Player Instance => m_instance ??= new();

    public PlayerData Data { get; private set; } = new();
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
  }
}