using UnityEngine;

namespace TRIdle.Game.Skill
{
  using System.Collections.Generic;
  using Logics.Math;

  public sealed class Effect_Common_Internalization : EffectBase<Effect_Common_Internalization>
  {
    public override string Name => Text.Common.Effect_Internalization_Name;
    public override string Description => Text.Common.Effect_Internalization_Description;
    public override SkillBase Category => Skill_Common.Instance;

    private readonly Dictionary<ActionBase, int> m_cumulated = new();

    private ActionBase Current => Player.Instance.Data.CurrentAction;
    public override void OnUpdate() {
      // TODO: Implement the logic for the effect.

      // Reduce all cumulated values, except for the current action
      if (m_cumulated.TryGetValue(Current, out _) is false) m_cumulated.Add(Current, 0);

      foreach (var action in m_cumulated.Keys) {
        if (action == Current) m_cumulated[action]++;
        else if (--m_cumulated[action] == 0) m_cumulated.Remove(action);
      }
    }

    private const int Yield = 30, Duration = 1800;
    private const float SpeedMultiplier = 0.2f;
    SFloat ModifyActionSpeed(SFloat value) {
      if (m_cumulated.TryGetValue(Current, out int count))
        value.multiplier += Mathf.Clamp(count - Yield, 0f, Duration) / Duration * SpeedMultiplier;
      return value;
    }

    public override void OnActivate() { }
    public override void OnDeactivate() { }
  }
}