using System.Collections;

using UnityEngine;

namespace TRIdle.Game
{
  using PlayerInternal;

  using Logics.Extensions;
  using Logics.Serialization;

  public class Player : LoaderBase
  {
    static Player m_instance;
    public static Player Instance => m_instance ??= new();

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
  
    #region Skill Actions
    PlayerMono m_mono;
    PlayerMono Mono {
      get {
        if (m_mono == null) {
          m_mono = new GameObject("PlayerMono").AddComponent<PlayerMono>();
          Object.DontDestroyOnLoad(m_mono.gameObject);
        }
        return m_mono;
      }
    }
    
    public void DoActionDelay(float delay, System.Action action)
      => Mono.StartDelay(delay, action);
    public void DoActionCooldown(float cooldown, System.Action action)
      => Mono.StartCooldown(cooldown, action);
    #endregion
  }

  public record PlayerData
  {

  }
}